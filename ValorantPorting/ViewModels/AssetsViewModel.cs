using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CUE4Parse.GameTypes.FN.Enums;
using CUE4Parse.UE4.AssetRegistry.Objects;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Exports.Engine;
using CUE4Parse.UE4.Assets.Exports.StaticMesh;
using CUE4Parse.UE4.Assets.Exports.Texture;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Objects.Core.i18N;
using CUE4Parse.UE4.Objects.Engine;
using CUE4Parse.UE4.Objects.GameplayTags;
using CUE4Parse.UE4.Objects.UObject;
using CUE4Parse.Utils;
using DynamicData;
using DynamicData.Binding;
using ValorantPorting.Application;
using ValorantPorting.Controls.Assets;
using ValorantPorting.Export;
using ValorantPorting.Extensions;
using ValorantPorting.Framework;
using ValorantPorting.Services;
using ValorantPorting.Framework.Services;
using Material.Icons;
using ReactiveUI;
using Serilog;
using DiscordRPC;
using SharpGLTF.Schema2;
using System.Drawing;
using static System.Runtime.InteropServices.JavaScript.JSType;
using CUE4Parse.GameTypes.PUBG.Assets.Exports;

namespace ValorantPorting.ViewModels;

public partial class AssetsViewModel : ViewModelBase
{
    public List<AssetLoader> Loaders;
    [ObservableProperty] private AssetLoader? currentLoader;
    [ObservableProperty] private Control expanderContainer;

    [ObservableProperty] private ObservableCollection<AssetOptions> currentAssets = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasCosmeticFilters))]
    [NotifyPropertyChangedFor(nameof(HasGameFilters))]
    private EAssetType currentAssetType;
    public bool HasCosmeticFilters => CosmeticFilterTypes.Contains(CurrentAssetType);
    private readonly EAssetType[] CosmeticFilterTypes =
    [
        EAssetType.Agent
    ];

    public bool HasGameFilters => GameFilterTypes.Contains(CurrentAssetType);
    private readonly EAssetType[] GameFilterTypes =
    [
        EAssetType.Rifle,
        EAssetType.GunBuddy
    ];

    [ObservableProperty] private int exportChunks;
    [ObservableProperty] private int exportProgress;
    [ObservableProperty] private bool isExporting;
    [ObservableProperty] private EExportTargetType exportType = EExportTargetType.Blender;
    [ObservableProperty] private ReadOnlyObservableCollection<AssetItem> activeCollection;

    [ObservableProperty] private ESortType sortType = ESortType.Default;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SortIcon))]
    private bool isDescending;

    [ObservableProperty] private string searchFilter = string.Empty;
    [ObservableProperty] private string filterPreviewText = "None";
    [ObservableProperty] private AvaloniaDictionary<string, Predicate<AssetItem>> filters = new();
    public MaterialIconKind SortIcon => IsDescending ? MaterialIconKind.SortDescending : MaterialIconKind.SortAscending;
    public readonly IObservable<SortExpressionComparer<AssetItem>> AssetSort;
    public readonly IObservable<Func<AssetItem, bool>> AssetFilter;

    public static readonly Dictionary<string, Predicate<AssetItem>> FilterPredicates = new()
    {
        { "Favorite", x => x.IsFavorite },
        { "Hidden Assets", x => x.Hidden }

    };

    public AssetsViewModel()
    {
        AssetFilter = this.WhenAnyValue(x => x.SearchFilter, x => x.Filters).Select(CreateAssetFilter);
        AssetSort = this.WhenAnyValue(x => x.SortType, x => x.IsDescending).Select(CreateAssetSort);
    }

    public void SetLoader(EAssetType assetType)
    {
        CurrentLoader = Loaders.First(x => x.Type == assetType);
        ActiveCollection = CurrentLoader.Target;
        SearchFilter = CurrentLoader.SearchFilter;
        CurrentAssetType = assetType;
        CurrentAssets.Clear();
    }

    public override async Task Initialize()
    {
        Loaders = new List<AssetLoader>
        {
            new(EAssetType.Agent)
            {
                Classes = new[] { "CharacterDataAsset" },
                IconHandler = asset =>
                {
                    asset.TryGetValue(out UTexture2D? previewImage, "DisplayIcon");
                    return previewImage;
                }
            },
            new(EAssetType.PlayerCard)
            {
                Classes = new[] { "PlayerCardDataAsset" }
            },
            new(EAssetType.Spray)
            {
                Classes = new[] { "SprayDataAsset" }
            },
            new(EAssetType.Melee)
            {
                Classes = new[] { "EquippableSkinDataAsset" },
                Types = new[] { "Melee" }
                //HidePredicate = (loader, asset, name) =>
                //{
                //    if (!AppSettings.Current.FilterItems) return false;
                //    var path = asset.GetPathName();
                //    var mappings = AppSettings.Current.ItemMeshMappings.GetOrAdd(name, () => new Dictionary<string, string>());
                //    if (mappings.TryGetValue(path, out var meshPath))
                //    {
                //        if (loader.LoadedAssetsForFiltering.Contains(meshPath)) return true;

                //        loader.LoadedAssetsForFiltering.Add(meshPath);
                //        return false;
                //    }

                //    var mesh = ExporterInstance.WeaponDefinitionMeshes(asset).FirstOrDefault();
                //    if (mesh is null) return true;

                //    meshPath = mesh.GetPathName();

                //    var shouldSkip = mappings.Any(x => x.Value.Equals(meshPath, StringComparison.OrdinalIgnoreCase));
                //    mappings[path] = meshPath;
                //    loader.LoadedAssetsForFiltering.Add(meshPath);
                //    return shouldSkip;
                //},
                //DontLoadHiddenAssets = true
            },
            new(EAssetType.MachineGun)
            {
                Classes = new[] { "EquippableSkinDataAsset" },
                Types = new[] { "HvyMachineGuns" }
            },
            new(EAssetType.Rifle)
            {
                Classes = new[] { "EquippableSkinDataAsset" },
                Types = new[] { "/Rifles" }
            },
            new(EAssetType.Shotgun)
            {
                Classes = new[] { "EquippableSkinDataAsset" },
                Types = new[] { "Shotguns" }
            },
            new(EAssetType.Sidearm)
            {
                Classes = new[] { "EquippableSkinDataAsset" },
                Types = new[] { "Sidearms" }
            },
            new(EAssetType.SniperRifle)
            {
                Classes = new[] { "EquippableSkinDataAsset" },
                Types = new[] { "SniperRifles" }
            },
            new(EAssetType.SMG)
            {
                Classes = new[] { "EquippableSkinDataAsset" },
                Types = new[] { "SubMachineGuns" }
            },
            new(EAssetType.GunBuddy)
            {
                Classes = new[] { "EquippableCharmDataAsset" }
            },
            new(EAssetType.Map)
            {
                Classes = new[] { "MapDataAsset" }
            },
            new(EAssetType.Bundle)
            {
                Classes = new[] { "StorefrontItemDataAsset" }
            }
        };

        SetLoader(EAssetType.Agent);
        TaskService.Run(async () => { await CurrentLoader!.Load(); });
    }

    public void ModifyFilters(string tag, bool enable)
    {
        if (!FilterPredicates.ContainsKey(tag)) return;

        if (enable)
            Filters.AddUnique(tag, FilterPredicates[tag]);
        else
            Filters.Remove(tag);

        FakeRefreshFilters();

        FilterPreviewText = Filters.Count > 0 ? Filters.Select(x => x.Key).CommaJoin(false) : "None";
    }

    [RelayCommand]
    public void Favorite()
    {
        foreach (var currentAsset in CurrentAssets) currentAsset.AssetItem.Favorite();
    }

    [RelayCommand]
    public async Task Export()
    {
        ExportChunks = 1;
        ExportProgress = 0;
        IsExporting = true;
        await ExportService.ExportAsync(CurrentAssets.ToList(), ExportType);
        IsExporting = false;
    }

    // scuffed fix to get filter to update
    private void FakeRefreshFilters()
    {
        var temp = Filters;
        Filters = null;
        Filters = temp;
    }

    protected static SortExpressionComparer<AssetItem> CreateAssetSort((ESortType, bool) values)
    {
        var (type, descending) = values;

        Func<AssetItem, IComparable> sort = type switch
        {
            ESortType.Default => asset => asset.ID,
            ESortType.AZ => asset => asset.DisplayName,
            _ => asset => asset.ID
        };

        return descending
            ? SortExpressionComparer<AssetItem>.Descending(sort)
            : SortExpressionComparer<AssetItem>.Ascending(sort);
    }

    protected static Func<AssetItem, bool> CreateAssetFilter((string, AvaloniaDictionary<string, Predicate<AssetItem>>?) values)
    {
        var (searchFilter, filters) = values;
        if (filters is null) return _ => true;
        return asset => asset.Match(searchFilter) && filters.All(x => x.Value.Invoke(asset)) && asset.Hidden == filters.ContainsKey("Hidden Assets");
    }
}

public partial class AssetLoader : ObservableObject
{
    [ObservableProperty, NotifyPropertyChangedFor(nameof(FinishedLoading))] private int loaded;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(FinishedLoading))] private int total;
    [ObservableProperty] private string searchFilter = string.Empty;

    public readonly EAssetType Type;
    public readonly WeaponsPauser Pause = new();
    public bool FinishedLoading => Loaded == Total;

    public readonly SourceList<AssetItem> Source = new();
    public readonly ReadOnlyObservableCollection<AssetItem> Target;
    public readonly ConcurrentBag<string> LoadedAssetsForFiltering = new();

    public string[] Classes = Array.Empty<string>();
    public string[] Types = Array.Empty<string>();
    public string[] Filters = Array.Empty<string>();
    public bool DontLoadHiddenAssets;
    public bool HideRarity;
    public Func<AssetLoader, UObject, string, bool> HidePredicate = (_, _, _) => false;
    public Func<UObject, UTexture2D?> IconHandler = asset => asset.GetAnyOrDefault<UTexture2D?>("DisplayIcon", "LevelSplashScreen");
    public Func<UObject, FText?> DisplayNameHandler = asset => asset.GetAnyOrDefault<FText?>("DisplayName", "ItemName") ?? new FText(asset.Name);
    public Func<AssetLoader, Task>? CustomLoadingHandler;

    private bool Started;

    public AssetLoader(EAssetType type)
    {
        Type = type;
        Source.Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Filter(AssetsVM.AssetFilter)
            .Sort(AssetsVM.AssetSort)
            .Bind(out Target)
            .Subscribe();
    }

    public async Task Load()
    {
        if (Started) return;
        Started = true;

        if (CustomLoadingHandler is not null)
        {
            await CustomLoadingHandler(this);
            return;
        }
        
        var assets = CUE4ParseVM.AssetRegistry.Where(data => !data.AssetName.Text.Contains("NPE") 
                                                                        && IsMatchingPrimaryTypeAsset(data) 
                                                                        && HasValidType(data)).ToList();

        Total = assets.Count;
        //foreach (var data in assets)
        //{
        //    await Pause.WaitIfPaused();
        //    try
        //    {
        //        await LoadAsset(data);
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Error("{0}", e);
        //    }
        //}
        await Parallel.ForEachAsync(assets, async (data, token) => //load if found
        {
            await LoadAsset(data);
        });

        Loaded = Total;
    }
    
    private bool IsMatchingPrimaryTypeAsset(FAssetData asset)
    {
        int tagCount = asset.TagsAndValues.Where(tag => Classes.Contains(tag.Value) && tag.Key.PlainText == "PrimaryAssetType").Count();
        return tagCount > 0;
    }

    private bool HasValidType(FAssetData asset)
    {
        return Types.Count() == 0 || Types.Where(type => asset.PackagePath.Text.Contains(type)).Count() > 0;
    }

    private async Task LoadAsset(FAssetData data)
    {
        var asset = await CUE4ParseVM.Provider.TryLoadObjectAsync(data.ObjectPath);
        if (asset is null) return;

        var displayName = data.AssetName.Text;
        if (data.TagsAndValues.TryGetValue("DisplayName", out var displayNameRaw)) displayName = displayNameRaw.SubstringBeforeLast('"').SubstringAfterLast('"').Trim();

        await LoadAsset(asset, displayName);
    }

    private async Task LoadAsset(UObject asset, string assetDisplayName)
    {
        Loaded++;

        var isHiddenAsset = Filters.Any(y => asset.Name.Contains(y, StringComparison.OrdinalIgnoreCase)) || HidePredicate(this, asset, assetDisplayName);
        if (isHiddenAsset && DontLoadHiddenAssets) return;

        var uBlueprintGeneratedClass = asset as UBlueprintGeneratedClass;
        asset = await uBlueprintGeneratedClass.ClassDefaultObject.LoadAsync<UObject>();

        UObject uiAsset = await UIDataService.GetUiDataAsync(asset);
        if (uiAsset is null) return;
        
        var icon = IconHandler(uiAsset);
        if (icon is null) return;
        //Log.Information("Type: {type}, Dimensions: {size}", Type.ToString(), icon.ImportedSize);

        var displayName = DisplayNameHandler(uiAsset)?.Text;
        if (string.IsNullOrEmpty(displayName)) displayName = asset.Name;

        await TaskService.RunDispatcherAsync(() => Source.Add(new AssetItem(asset, uiAsset, icon, displayName, Type, isHiddenAsset, HideRarity)), DispatcherPriority.Background);
    }
}

public class WeaponsPauser
{
    public bool IsPaused;

    public void Pause()
    {
        IsPaused = true;
    }

    public void Unpause()
    {
        IsPaused = false;
    }

    public async Task WaitIfPaused()
    {
        while (IsPaused) await Task.Delay(1);
    }
}

file class ManualAssetItemEntry
{
    public string Name;
    public UObject Mesh;
    public UTexture2D PreviewImage;

    public static async Task<ManualAssetItemEntry> Create(string name, string meshPath, string imagePath)
    {
        return new ManualAssetItemEntry
        {
            Name = name,
            Mesh = await CUE4ParseVM.Provider.LoadObjectAsync(meshPath),
            PreviewImage = await CUE4ParseVM.Provider.LoadObjectAsync<UTexture2D>(imagePath)
        };
    }
}