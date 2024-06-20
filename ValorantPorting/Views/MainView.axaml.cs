using System;
using Avalonia.Controls;
using ValorantPorting.Application;
using ValorantPorting.Framework;
using ValorantPorting.ViewModels;

namespace ValorantPorting.Views;

public partial class MainView : ViewBase<MainViewModel>
{
    public MainView()
    {
        InitializeComponent();
    }

    private void OnTabChanged(object? sender, EventArgs e)
    {
        ViewModel.ActiveTab = (UserControl) sender!;
        MainWindow.Width = ViewModel.ActiveTab is AssetsView or FilesView && MainWindow.WindowState == WindowState.Normal ? 1280 : 1100; // assets view and files view use this specifically
        MainWindow.UpdateLayout();
    }
}