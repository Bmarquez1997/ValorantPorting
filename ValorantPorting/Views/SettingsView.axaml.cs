using ValorantPorting.Application;
using ValorantPorting.Framework;
using ValorantPorting.ViewModels;

namespace ValorantPorting.Views;

public partial class SettingsView : ViewBase<SettingsViewModel>
{
    public SettingsView() : base(AppSettings.Current)
    {
        InitializeComponent();
    }
}