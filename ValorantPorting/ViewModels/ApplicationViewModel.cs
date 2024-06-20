using System;
using System.Collections.Generic;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using ValorantPorting.Application;
using ValorantPorting.Extensions;
using ValorantPorting.Framework;
using ValorantPorting.Framework.Application;
using ValorantPorting.Framework.Controls;
using ValorantPorting.Framework.ViewModels;
using ValorantPorting.Views;

namespace ValorantPorting.ViewModels;

public partial class ApplicationViewModel : ViewModelBase
{
    [ObservableProperty] private ThemedViewModelBase theme;
    [ObservableProperty] private string versionString = $"v{Globals.VersionString}";

    [ObservableProperty] private UserControl? currentView;

    public ApplicationViewModel()
    {
        Theme = ThemeVM;
        ThemeVM.UseMicaBackground = AppSettings.Current.UseMica;
        ThemeVM.BackgroundColor = AppSettings.Current.BackgroundColor;
        ColorExtensions.SetSystemAccentColor(AppSettings.Current.AccentColor);
        
        switch (AppSettings.Current.LoadingType)
        {
            case ELoadingType.Live:
            case ELoadingType.Local when AppSettings.Current.HasValidLocalData:
                SetView<MainView>();
                break;
            default:
                SetView<WelcomeView>();
                break;
        }
    }

    public void SetView<T>() where T : UserControl, new()
    {
        CurrentView = new T();
    }

    public void RestartWithMessage(string caption, string message, bool mandatory = true)
    {
        var restartButton = new MessageWindowButton("Restart", _ => Restart());
        var waitButton = new MessageWindowButton("Wait", window => window.Close());
        var buttons = mandatory ? new List<MessageWindowButton> { restartButton } : [ restartButton, waitButton ];
        MessageWindow.Show(caption, message, buttons);
    }

    public void Restart()
    {
        Launch(AppDomain.CurrentDomain.FriendlyName, false);
        Shutdown();
    }

    public void Shutdown()
    {
        AppBase.Application.Shutdown();
    }
}