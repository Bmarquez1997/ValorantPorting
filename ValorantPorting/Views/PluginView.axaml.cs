using ValorantPorting.Application;
using ValorantPorting.Framework;
using ValorantPorting.ViewModels;

namespace ValorantPorting.Views;

public partial class PluginView : ViewBase<PluginViewModel>
{
    public PluginView() : base(AppSettings.Current.Plugin)
    {
        InitializeComponent();
    }
}