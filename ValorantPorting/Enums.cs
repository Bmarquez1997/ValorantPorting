using System.ComponentModel;
using ValorantPorting.Extensions;

namespace ValorantPorting;

public enum ELoadingType
{
    [Description("Local (Installed)")]
    Local,

    [Description("Live (On-Demand)")]
    Live
}

public enum EAssetType
{
    [Description("None")]
    None,

    // Valorant Specific 
    // TODO: Add sizes to enum
    // TODO: Add menu group enum?
    // Todo: Styles

    [Description("Agents"), Export(EExportType.Mesh), ViewDimensions(70, 80)]
    Agent,

    [Description("Player Cards"), Export(EExportType.Texture), ViewDimensions(70, 80)]
    PlayerCard,

    [Description("Sprays"), Export(EExportType.Texture), ViewDimensions(70, 80)]
    Spray,

    [Description("Machine Gun"), Export(EExportType.Mesh), ViewDimensions(170, 60)]
    MachineGun,

    [Description("Melee"), Export(EExportType.Mesh), ViewDimensions(130, 60)]
    Melee,

    [Description("Rifle"), Export(EExportType.Mesh), ViewDimensions(170, 60)]
    Rifle,

    [Description("Shotgun"), Export(EExportType.Mesh), ViewDimensions(170, 60)]
    Shotgun,

    [Description("Sidearm"), Export(EExportType.Mesh), ViewDimensions(90, 70)]
    Sidearm,

    [Description("SMG"), Export(EExportType.Mesh), ViewDimensions(170, 60)]
    SMG,

    [Description("Sniper Rifle"), Export(EExportType.Mesh), ViewDimensions(200, 60)]
    SniperRifle,

    [Description("Gunbuddies"), Export(EExportType.Mesh), ViewDimensions(80, 80)]
    GunBuddy,

    [Description("Maps"), Export(EExportType.Mesh), ViewDimensions(150, 150)]
    Map,

    [Description("Bundles"), Export(EExportType.Mesh), ViewDimensions(170, 60)]
    Bundle,

    // GENERIC

    [Description("Mesh"), Export(EExportType.Mesh)]
    Mesh,
    
    [Description("World"), Export(EExportType.Mesh)]
    World,
    

    [Description("Texture"), Export(EExportType.Texture)]
    Texture,
    
    [Description("Animation"), Export(EExportType.Animation)]
    Animation,
    
    [Description("Sound"), Export(EExportType.Sound)]
    Sound
    
}

public enum EExportType
{
    [Description("Mesh")]
    Mesh,

    [Description("Animation")]
    Animation,

    [Description("Texture")]
    Texture,

    // [Description("Map")]
    // Map,
    
    [Description("Sound")]
    Sound
}


public enum EExportTargetType
{
    [Description("Blender")]
    Blender,

    [Description("Unreal Engine")]
    Unreal,

    [Description("Folder")]
    Folder
}

public enum ESortType
{
    [Description("Default")]
    Default,

    [Description("A-Z")]
    AZ,

    [Description("Season")]
    Season,

    [Description("Rarity")]
    Rarity
}

public enum EWeaponType
{
    [Description("Attatchment")]
    Attatchment,
    [Description("RealWeapon")]
    RealWeapon,
}

public enum EImageType
{
    [Description("PNG (.png)")]
    PNG,

    [Description("Targa (.tga)")]
    TGA
}

public enum ERigType
{
    [Description("Default Rig (FK)")]
    Default,

    [Description("Valorant Rig (IK)")]
    Valorant
}

public enum ESupportedLODs
{
    [Description("LOD 0 (Highest Quality)")]
    LOD0,

    [Description("LOD 1")]
    LOD1,

    [Description("LOD 2")]
    LOD2,

    [Description("LOD 3")]
    LOD3,

    [Description("LOD 4 (Lowest Quality)")]
    LOD4
}

public enum EMeshExportTypes
{
    [Description(".uemodel")]
    UEFormat,

    [Description(".psk")]
    ActorX
}

public enum EAnimExportTypes
{
    [Description(".ueanim")]
    UEFormat,

    [Description(".psa")]
    ActorX
}

public enum ETextureExportTypes
{
    [Description("Blend Data")]
    Data,

    [Description("Images as Planes")]
    Plane
}

public enum EAssetSize
{
    [Description("50%")]
    Percent50,
    
    [Description("75%")]
    Percent75,
    
    [Description("100%")]
    Percent100,
    
    [Description("125%")]
    Percent125,
    
    [Description("150%")]
    Percent150,
    
    [Description("175%")]
    Percent175,
    
    [Description("200%")]
    Percent200
}