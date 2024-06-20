using ValorantPorting.Framework.ViewModels;
using ValorantPorting.Framework.ViewModels.Endpoints;
using ValorantPorting.ViewModels.Endpoints;

namespace ValorantPorting.ViewModels;

public class EndpointViewModel : EndpointViewModelBase
{
    public readonly ValorantCentralEndpoint ValorantCentral;
    public readonly ValorantPortingEndpoint ValorantPorting;
    public readonly EpicGamesEndpoint EpicGames;
    
    public EndpointViewModel() : base($"ValorantPorting/{Globals.VersionString}")
    {
        ValorantCentral = new ValorantCentralEndpoint(Client);
        ValorantPorting = new ValorantPortingEndpoint(Client);
        EpicGames = new EpicGamesEndpoint(Client);
    }
}