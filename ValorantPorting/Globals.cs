global using static ValorantPorting.Application.App;
global using static ValorantPorting.Framework.Application.AppBase;
using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse.UE4.Versions;
using ValorantPorting.Framework.ViewModels.Endpoints.Models;

namespace ValorantPorting;

public static class Globals
{
    public static readonly VPVersion Version = new(2, 0, 1);
    public static readonly string VersionString = Version.ToString();

    public const string DISCORD_URL = "https://discord.gg/DZ5YFXdBA6";
    public const string GITHUB_URL = "https://github.com/KaiserM21/ValorantPorting";
    public const string KOFI_URL = "https://ko-fi.com/halfuwu";
    public const string WIKI_URL = "https://github.com/KaiserM21/ValorantPorting/wiki";

    public static readonly FGuid ZERO_GUID = new();
    public const string ZERO_CHAR = "0x0000000000000000000000000000000000000000000000000000000000000000";
    public const string KEY_STRING = "0x4BE71AF2459CF83899EC9DC2CB60E22AC4B3047E0211034BBABE9D174C069DD6";
    public const EGame LatestGameVersion = EGame.GAME_Valorant;
}