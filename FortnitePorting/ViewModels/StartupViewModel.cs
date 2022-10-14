﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CUE4Parse.UE4.Versions;
using FortnitePorting.AppUtils;
using Newtonsoft.Json;
using Serilog;

namespace FortnitePorting.ViewModels;

public class StartupViewModel : ObservableObject
{
    public string ArchivePath
    {
        get => AppSettings.Current.ArchivePath;
        set
        {
            AppSettings.Current.ArchivePath = value;
            OnPropertyChanged();
        }
    }
    
    public ELanguage Language
    {
        get => AppSettings.Current.Language;
        set
        {
            AppSettings.Current.Language = value;
            OnPropertyChanged();
        }
    }
    
    public void CheckForInstallation()
    {
        LauncherInstalled? launcherInstalled = null;
        foreach (var drive in DriveInfo.GetDrives())
        {
            var launcherInstalledPath = $"{drive.Name}ProgramData\\Epic\\UnrealEngineLauncher\\LauncherInstalled.dat";
            if (!File.Exists(launcherInstalledPath)) continue;

            launcherInstalled = JsonConvert.DeserializeObject<LauncherInstalled>(File.ReadAllText(launcherInstalledPath));
        }
        if (launcherInstalled is null) return;

        var fortniteInfo = launcherInstalled.InstallationList.FirstOrDefault(x => x.AppName.Equals("Fortnite"));
        if (fortniteInfo is null) return;

        ArchivePath = fortniteInfo.InstallLocation + "\\ShooterGame\\Content\\Paks\\";
        Log.Information("Detected EGL Installation at {0}", ArchivePath);
    }

    private class LauncherInstalled
    {
        public List<LauncherInstalledInfo> InstallationList;
        public class LauncherInstalledInfo
        {
            public string InstallLocation;
            public string NamespaceId; // useless
            public string ItemId; // useless
            public string ArtifactId; // useless
            public string AppVersion;
            public string AppName;
        }
    }
}