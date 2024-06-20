using ValorantPorting.Application;
using ValorantPorting.Framework;
using ValorantPorting.ViewModels;

namespace ValorantPorting.Views;

public partial class ExportOptionsView : ViewBase<ExportOptionsViewModel>
{
    public ExportOptionsView() : base(AppSettings.Current.ExportOptions)
    {
        InitializeComponent();
    }
}