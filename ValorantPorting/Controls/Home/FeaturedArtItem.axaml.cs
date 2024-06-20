using Avalonia.Controls;
using Avalonia.Interactivity;
using ValorantPorting.Framework.ViewModels.Endpoints.Models;
using ValorantPorting.ViewModels.Endpoints.Models;

namespace ValorantPorting.Controls.Home;

public partial class FeaturedArtItem : UserControl
{
    public string Artist { get; set; }
    public string ImageURL { get; set; }
    public string SocialsURL { get; set; }

    public FeaturedArtItem(FeaturedResponse featured)
    {
        InitializeComponent();

        Artist = featured.Artist;
        ImageURL = featured.ImageURL;
        SocialsURL = featured.SocialsURL;
    }

    private void OnSocialsButtonClicked(object? sender, RoutedEventArgs e)
    {
        Launch(SocialsURL);
    }
}