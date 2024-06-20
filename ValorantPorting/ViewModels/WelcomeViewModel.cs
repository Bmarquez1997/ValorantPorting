using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CUE4Parse.UE4.Versions;
using CUE4Parse.Utils;
using ValorantPorting.Application;
using ValorantPorting.Framework;
using ValorantPorting.Framework.Application;
using ValorantPorting.Views;
using Newtonsoft.Json;
using Serilog;
using J = Newtonsoft.Json.JsonPropertyAttribute;

namespace ValorantPorting.ViewModels;

public partial class WelcomeViewModel : ViewModelBase
{
    public bool CanContinue => CurrentLoadingType switch
    {
        ELoadingType.Local => !string.IsNullOrWhiteSpace(LocalArchivePath) && Directory.Exists(LocalArchivePath),
        ELoadingType.Live => true
    };

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(CanContinue))]
    private ELoadingType currentLoadingType;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(CanContinue))]
    private string localArchivePath;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(CanContinue))]
    private string customArchivePath;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(CanContinue))]
    private string customMappingsPath;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(CanContinue))]
    private string customEncryptionKey = Globals.ZERO_CHAR;

    [ObservableProperty] private EGame customUnrealVersion = Globals.LatestGameVersion;

    private static readonly FilePickerFileType MappingsFileType = new("Unreal Mappings")
    {
        Patterns = new[] { "*.usmap" }
    };

    public override async Task Initialize()
    {
        await CheckForInstallation();
    }

    private async Task CheckForInstallation()
    {
        LauncherInstalled? launcherInstalled = null;
        foreach (var drive in DriveInfo.GetDrives())
        {
            var launcherInstalledPath = $"{drive.Name}ProgramData\\Riot Games\\VALORANT\\live\\LauncherInstalled.dat";
            if (!File.Exists(launcherInstalledPath)) continue;

            launcherInstalled = JsonConvert.DeserializeObject<LauncherInstalled>(await File.ReadAllTextAsync(launcherInstalledPath));
        }

        var valorantInfo = launcherInstalled?.InstallationList.FirstOrDefault(x => x.AppName.Equals("Valorant"));
        if (valorantInfo is null) return;

        LocalArchivePath = valorantInfo.InstallLocation + "\\ShooterGame\\Content\\Paks\\";
        Log.Information("Found Valorant Installation at {ArchivePath}", LocalArchivePath);
    }

    [RelayCommand]
    private async Task BrowseLocalArchivePath()
    {
        if (await BrowseFolderDialog() is { } path) LocalArchivePath = path;
    }

    [RelayCommand]
    private async Task BrowseCustomArchivePath()
    {
        if (await BrowseFolderDialog() is { } path) CustomArchivePath = path;
    }

    [RelayCommand]
    private async Task BrowseMappingsFile()
    {
        if (await BrowseFileDialog(MappingsFileType) is { } path) CustomMappingsPath = path;
    }

    [RelayCommand]
    private void Continue()
    {
        AppSettings.Current.LoadingType = CurrentLoadingType;
        AppSettings.Current.LocalArchivePath = LocalArchivePath;
        AppSettings.Current.CustomArchivePath = CustomArchivePath;
        AppSettings.Current.CustomEncryptionKey = CustomEncryptionKey;
        AppSettings.Current.CustomMappingsPath = CustomMappingsPath;

        AppVM.SetView<MainView>();
    }
}

public class LauncherInstalled
{
    [J] public List<LauncherInstalledInfo> InstallationList;
}

public class LauncherInstalledInfo
{
    [J] public string InstallLocation;
    [J] public string AppVersion;
    [J] public string AppName;
}