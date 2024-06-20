using System;
using System.IO;
using ValorantPorting.ViewModels;
using Newtonsoft.Json;

namespace ValorantPorting.Application;

public static class AppSettings
{
    public static SettingsViewModel Current = new();

    private static readonly DirectoryInfo DirectoryPath = new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ValorantPorting"));
    private static readonly FileInfo FilePath = new(Path.Combine(DirectoryPath.FullName, "DEV_AppSettingsV2.json"));

    public static void Load()
    {
        if (!DirectoryPath.Exists) DirectoryPath.Create();
        if (!FilePath.Exists) return;
        Current = JsonConvert.DeserializeObject<SettingsViewModel>(File.ReadAllText(FilePath.FullName)) ?? new SettingsViewModel();
    }

    public static void Save()
    {
        File.WriteAllText(FilePath.FullName, JsonConvert.SerializeObject(Current, Formatting.Indented));
    }
}