using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CUE4Parse_Conversion.Textures;
using CUE4Parse.GameTypes.FN.Enums;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Exports.Texture;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Objects.Core.i18N;
using CUE4Parse.UE4.Objects.GameplayTags;
using CUE4Parse.UE4.Objects.UObject;
using CUE4Parse.Utils;
using ValorantPorting.Application;
using ValorantPorting.Extensions;
using ValorantPorting.ViewModels;
using SkiaSharp;
using SkiaExtensions = ValorantPorting.Extensions.SkiaExtensions;

namespace ValorantPorting.Controls.Assets;

public partial class AssetItem : UserControl
{
    private static readonly EAssetType[] wideDisplay = [
        EAssetType.Rifle,
        EAssetType.Shotgun,
        EAssetType.SMG,
        EAssetType.MachineGun,
        EAssetType.SniperRifle
        ];

    private static readonly EAssetType[] squareDisplay = [
        EAssetType.Sidearm,
        EAssetType.Melee
        ];

    public static readonly StyledProperty<bool> IsFavoriteProperty = AvaloniaProperty.Register<AssetItem, bool>(nameof(IsFavoriteProperty));

    public bool IsFavorite
    {
        get => GetValue(IsFavoriteProperty);
        set => SetValue(IsFavoriteProperty, value);
    }

    public bool Hidden { get; set; }
    public EAssetType Type { get; set; }
    public UObject Asset { get; set; }
    public string ID { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }

    public FGameplayTagContainer? GameplayTags { get; set; }
    public EFortRarity Rarity { get; set; }
    public int Season { get; set; }
    public string Series { get; set; }

    public Bitmap IconBitmap { get; set; }
    public Bitmap PreviewImage { get; set; }
    
    public float DisplayWidth { get; set; }
    public float DisplayHeight { get; set; }
    

    public AssetItem(UObject asset, UObject uiAsset, UTexture2D icon, string displayName, EAssetType type, bool isHidden = false, bool hideRarity = false, EFortRarity? rarityOverride = null, bool useTitleCase = true)
    {
        DataContext = this;
        InitializeComponent();

        Hidden = isHidden;
        Type = type;
        Asset = asset;
        IsFavorite = AppSettings.Current.FavoritePaths.Contains(asset.GetPathName());
        //var idObj = asset.GetAnyOrDefault<Byte?>("CharacterID", "Uuid"); TODO
        ID =  asset.Name;
        DisplayName = useTitleCase ? displayName.TitleCase() : displayName;
        var description = uiAsset.GetAnyOrDefault<FText?>("Description", "ItemDescription") ?? new FText("No description.");
        Description = description.Text;
        Rarity = rarityOverride ?? asset.GetOrDefault("Rarity", EFortRarity.Uncommon);
        GameplayTags = asset.GetOrDefault<FGameplayTagContainer?>("GameplayTags");
        //if (type is EAssetType.Prefab)
        //{
        //    var tagsHelper = asset.GetOrDefault<FStructFallback?>("CreativeTagsHelper");
        //    var tags = tagsHelper?.GetOrDefault("CreativeTags", Array.Empty<FName>()) ?? Array.Empty<FName>();
        //    var gameplayTags = tags.Select(tag => new FGameplayTag(tag)).ToArray();
        //    GameplayTags = new FGameplayTagContainer(gameplayTags);
        //}

        //TODO: Make wider for weapons
        if (type.HasDimensions())
        {
            DisplayWidth = type.GetWidth() * AppSettings.Current.AssetSizeMultiplier;
            DisplayHeight = type.GetHeight() * AppSettings.Current.AssetSizeMultiplier;
        }
        else
        {
            DisplayWidth = 64 * AppSettings.Current.AssetSizeMultiplier;
            DisplayHeight = 80 * AppSettings.Current.AssetSizeMultiplier;
        }

        var seasonTag = GameplayTags?.GetValueOrDefault("Cosmetics.Filter.Season.")?.Text;
        Season = int.TryParse(seasonTag?.SubstringAfterLast("."), out var seasonNumber) ? seasonNumber : int.MaxValue;

        var series = Asset.GetOrDefault<UObject?>("Series");
        Series = series?.GetAnyOrDefault<FText>("DisplayName", "ItemName").Text ?? string.Empty;

        var iconBitmap = icon.Decode()!;
        if (iconBitmap != null) //TODO: Remove
        {
            IconBitmap = new Bitmap(iconBitmap.Encode(SKEncodedImageFormat.Png, 100).AsStream());

            var fullBitmap = new SKBitmap(iconBitmap.Width, iconBitmap.Height, iconBitmap.ColorType, iconBitmap.AlphaType);
            using (var fullCanvas = new SKCanvas(fullBitmap))
            {
                //TODO: Figure out how to fill background
                // fullCanvas.DrawRect(new SKRect(0, 0, getAssetWidth(iconBitmap), getAssetHeight(iconBitmap)), new SKPaint
                // {
                //     Shader = SkiaExtensions.RadialGradient(iconBitmap.Width, SKColor.Parse("#364966"), SKColor.Parse("#30383f"))
                // });

                fullCanvas.DrawBitmap(iconBitmap, new SKRect(0, 0, fullBitmap.Width, fullBitmap.Height));
            }

            PreviewImage = new Bitmap(fullBitmap.Encode(SKEncodedImageFormat.Png, 100).AsStream());
        }
    }

    //TODO: Fix to properly fill squares
    public int getAssetWidth(SKBitmap iconBitmap)
    {
        if (DisplayWidth > iconBitmap.Width)
        {
            return DisplayWidth.TruncToInt() + 1;
        }
        else
        {
            return ((DisplayWidth / DisplayHeight) * iconBitmap.Height).TruncToInt() + 1;
        }
    }

    public int getAssetHeight(SKBitmap iconBitmap)
    {
        if (DisplayHeight > iconBitmap.Height)
        {
            return DisplayHeight.TruncToInt() + 1;
        }
        else
        {
            return ((DisplayHeight / DisplayWidth) * iconBitmap.Width).TruncToInt() + 1;
        }
    }

    public bool Match(string filter)
    {
        return MiscExtensions.Filter(DisplayName, filter) || MiscExtensions.Filter(ID, filter);
    }

    public void ChangeSize(float multiplier)
    {
        
    }

    public void Favorite()
    {
        var path = Asset.GetPathName();
        if (!AppSettings.Current.FavoritePaths.Contains(path))
        {
            AppSettings.Current.FavoritePaths.AddUnique(path);
            IsFavorite = true;
        }
        else
        {
            AppSettings.Current.FavoritePaths.Remove(path);
            IsFavorite = false;
        }
    }

    public void CopyPath()
    {
        Clipboard.SetTextAsync(Asset.GetPathName());
    }

    public void CopyID()
    {
        Clipboard.SetTextAsync(Asset.Name);
    }
}