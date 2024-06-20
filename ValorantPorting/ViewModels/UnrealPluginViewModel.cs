using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CUE4Parse.Utils;
using ValorantPorting.Controls;
using ValorantPorting.Extensions;
using ValorantPorting.Framework;
using ValorantPorting.Framework.Controls;
using ValorantPorting.Services;
using ValorantPorting.Framework.Services;
using Serilog;

namespace ValorantPorting.ViewModels;

public partial class UnrealPluginViewModel : ViewModelBase
{
    [ObservableProperty] private bool automaticUpdate = true;
    [ObservableProperty] private ObservableCollection<UnrealProjectInfo> projects = new();
    
    private static readonly FilePickerFileType FileType = new("Unreal")
    {
        Patterns = new[] { "*.uproject" }
    };

    public async Task Add()
    {
        var path = await BrowseFileDialog(FileType);
        if (path is null) return;
        
        if (CheckUnrealRunning(path)) return;

        var versionInfo = FileVersionInfo.GetVersionInfo(path);
        if (Projects.Any(x => x.UnrealVersion.Equals(versionInfo.ProductVersion))) return;
        var majorVersion = int.Parse(versionInfo.ProductVersion[..1]);
        if (majorVersion < 4)
        {
            MessageWindow.Show("Invalid Unreal Engine Version", "Only Unreal Engine versions 5.2 or higher are supported.");
            return;
        }

        var installInfo = new UnrealProjectInfo(path, versionInfo.ProductVersion);
        await Sync(installInfo);
        await TaskService.RunDispatcherAsync(() => Projects.Add(installInfo));
    }
    
    public async Task Remove(UnrealProjectInfo removeItem)
    {
        Projects.Remove(removeItem);
        await UnSync(removeItem);
    }

    public async Task SyncAll(bool automatic = false)
    {
        foreach (var unrealInstall in Projects)
        {
            await Sync(unrealInstall, automatic);
        }
    }
    
    public async Task Sync(UnrealProjectInfo installInfo, bool automatic = false)
    {
        installInfo.Update();
        
        var currentPluginVersion = await GetPluginVersion();
        if (CheckUnrealRunning(installInfo.UnrealPath, automatic))
        {
            if (!installInfo.PluginVersion.Equals(currentPluginVersion) && automatic)
            {
                MessageWindow.Show("An Error Occurred", $"ValorantPorting tried to auto sync the plugin, but an instance of unreal is open. Please close it and sync the plugin in the plugin tab.");
            }
            return;
        }
        
        if (installInfo.PluginVersion.Equals(currentPluginVersion) && automatic) return;
        
        var assets = Avalonia.Platform.AssetLoader.GetAssets(new Uri("avares://ValorantPorting/Plugins/Unreal"), null);
        foreach (var asset in assets)
        {
            var filePath = Path.Combine(installInfo.AddonPath, asset.AbsolutePath.SubstringAfterLast("/"));
            var assetStream = Avalonia.Platform.AssetLoader.Open(asset);
            await File.WriteAllBytesAsync(filePath, assetStream.ReadToEnd());
        }
        installInfo.Update();
        
        Log.Information("Synced Unreal {UnrealVersion} Plugin to Version {PluginVersion}", installInfo.UnrealVersion, installInfo.PluginVersion);
        MessageWindow.Show("Sync Successful", $"Successfully synced the Unreal {installInfo.UnrealVersion} plugin to version {installInfo.PluginVersion}. If this is your first time installing the plugin, please ensure the plugin is enabled in Unreal.");

        try
        {
            using var unrealProcess = new Process();
            unrealProcess.StartInfo = new ProcessStartInfo
            {
                FileName = installInfo.UnrealPath,
                Arguments = $"-b --python-exit-code 1 --python \"{DependencyService.BlenderScriptFile.FullName}\"",
                UseShellExecute = false
            };

            unrealProcess.Exited += (sender, args) =>
            {
                if (automatic) return;
                
                if (sender is Process { ExitCode: 1 } process)
                {
                    MessageWindow.Show("An Error Occured", "Unreal failed to enable the ValorantPorting plugin. If this is your first time using syncing the plugin, please enable it yourself in the add-ons tab in Unreal preferences, otherwise, you may ignore this message.");
                    Log.Error(process.StandardOutput.ReadToEnd());
                }
            };

            unrealProcess.Start();
        }
        catch (Exception e)
        {
            MessageWindow.Show("An Error Occured", "Unreal failed to enable the ValorantPorting plugin. If this is your first time using syncing the plugin, please enable it yourself in the add-ons tab in Unreal preferences, otherwise, you may ignore this message.");
            Log.Error(e.ToString());
        }
      
    }
    
    public async Task UnSync(UnrealProjectInfo installInfo)
    {
        Directory.Delete(installInfo.AddonPath);
    }
    
    public bool CheckUnrealRunning(string path, bool automatic = false)
    {
        var unrealProcesses = Process.GetProcessesByName("unreal");
        var foundProcess = unrealProcesses.FirstOrDefault(process => process.MainModule?.FileName.Equals(path.Replace("/", "\\")) ?? false);
        if (foundProcess is not null)
        {
            if (!automatic)
            {
                MessageWindow.Show("Cannot Sync Plugin", 
                    $"An instance of unreal is open. Please close it to sync the plugin.\n\nPath: \"{path}\"\nPID: {foundProcess.Id}",
                    buttons: [new MessageWindowButton("Kill Process", window =>
                    {
                        foundProcess.Kill();
                        window.Close();
                    }), MessageWindowButtons.Continue]);
            }
           
            return true;
        }

        return false;
    }

    public async Task<string> GetPluginVersion()
    {
        var initStream = Avalonia.Platform.AssetLoader.Open(new Uri("avares://ValorantPorting/Plugins/Unreal/__init__.py"));
        var initText = initStream.ReadToEnd().BytesToString();
        return UnrealProjectInfo.GetPluginVersion(initText);
    }
}

public partial class UnrealProjectInfo : ObservableObject
{
    [ObservableProperty] private string unrealPath;
    [ObservableProperty] private string unrealVersion;
    [ObservableProperty] private string pluginVersion = "???";
    [ObservableProperty] private string addonBasePath;
    [ObservableProperty] private string addonPath;

    public UnrealProjectInfo(string path, string unrealVersion)
    {
        UnrealPath = path;
        UnrealVersion = unrealVersion;
        AddonBasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
            "Unreal Foundation", 
            "Unreal", 
            UnrealVersion, 
            "scripts", 
            "addons");
        AddonPath = Path.Combine(AddonBasePath, "ValorantPorting");
        Directory.CreateDirectory(AddonPath);
    }

    public void Update()
    {
        PluginVersion = GetPluginVersion();
    }

    public string GetPluginVersion()
    {
        var initFilepath = Path.Combine(AddonPath, "__init__.py");
        if (!File.Exists(initFilepath)) return PluginVersion;
        
        var initText = File.ReadAllText(initFilepath);
        return GetPluginVersion(initText);
    }

    public static string GetPluginVersion(string text)
    {
        var versionMatch = Regex.Match(text, @"""version"": \((.*)\)");
        return versionMatch.Groups[^1].Value.Replace(", ", ".").Replace("\"", string.Empty);
    }
}