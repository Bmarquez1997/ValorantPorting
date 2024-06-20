using Avalonia.Controls;
using ValorantPorting.Framework;
using ValorantPorting.ViewModels;

namespace ValorantPorting.Views;

public partial class WelcomeView : ViewBase<WelcomeViewModel>
{
    public WelcomeView()
    {
        InitializeComponent();
    }

    private void OnTabSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not TabControl tabControl) return;
        if (tabControl.SelectedItem is not TabItem tabItem) return;
        if (tabItem.Tag is not ELoadingType loadingType) return;
        ViewModel.CurrentLoadingType = loadingType;
    }
}