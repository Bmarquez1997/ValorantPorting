﻿using System;
using System.Windows;
using AdonisUI.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using ValorantPorting.AppUtils;
using MessageBox = AdonisUI.Controls.MessageBox;
using MessageBoxImage = AdonisUI.Controls.MessageBoxImage;

namespace ValorantPorting.ViewModels;

public class ApplicationViewModel : ObservableObject
{
    public AssetHandlerViewModel? AssetHandlerVM;
    public BlenderViewModel BlenderVM;
    public CUE4ParseViewModel CUE4ParseVM;
    public MainViewModel MainVM;
    public SettingsViewModel SettingsVM;
    public StartupViewModel StartupVM;

    public void RestartWithMessage(string caption, string message)
    {
        var messageBox = new MessageBoxModel
        {
            Caption = caption,
            Icon = MessageBoxImage.Exclamation,
            Text = message,
            Buttons = new[] { MessageBoxButtons.Ok() }
        };

        MessageBox.Show(messageBox);
        Restart();
    }

    public void Restart()
    {
        AppHelper.Launch(AppDomain.CurrentDomain.FriendlyName, false);
        Application.Current.Shutdown();
    }

    public void Quit()
    {
        Application.Current.Shutdown();
    }
}