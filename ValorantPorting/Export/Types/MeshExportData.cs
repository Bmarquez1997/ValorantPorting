using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CUE4Parse_Conversion.Meshes;
using CUE4Parse.GameTypes.FN.Assets.Exports;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Exports.Animation;
using CUE4Parse.UE4.Assets.Exports.Component.SkeletalMesh;
using CUE4Parse.UE4.Assets.Exports.Component.StaticMesh;
using CUE4Parse.UE4.Assets.Exports.Engine;
using CUE4Parse.UE4.Assets.Exports.Material;
using CUE4Parse.UE4.Assets.Exports.SkeletalMesh;
using CUE4Parse.UE4.Assets.Exports.StaticMesh;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Objects.Core.Math;
using CUE4Parse.UE4.Objects.Engine;
using CUE4Parse.UE4.Objects.GameplayTags;
using CUE4Parse.UE4.Objects.UObject;
using CUE4Parse.Utils;
using ValorantPorting.Application;
using ValorantPorting.Extensions;
using Serilog;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ValorantPorting.Export.Types;

public class MeshExportData : ExportDataBase
{
    public readonly List<ExportMesh> Meshes = new();
    public readonly List<ExportMesh> OverrideMeshes = new();
    public readonly List<ExportOverrideMaterial> OverrideMaterials = new();
    public readonly List<ExportOverrideParameters> OverrideParameters = new();
    public readonly AnimExportData Animation;

    public MeshExportData(string name, UObject asset, List<UObject> styles, EAssetType type, EExportTargetType exportType) : base(name, asset, styles, type, EExportType.Mesh, exportType)
    {
        switch (type)
        {
            case EAssetType.Agent:
            {
                // V1 export code:
                // var meshes = new UObject[3];
                // asset.TryGetValue(out meshes[0], "MeshOverlay1P");
                // if (meshes[0].Properties.Count < 2)
                // {
                //     asset.TryGetValue(out meshes[0], "Mesh1P");
                // }
                // asset.TryGetValue(out meshes[1], "MeshCosmetic3P");
                // meshes[2] = ExportHelpers.GetCsMesh(asset);
                // List<ExportPart> parts = new List<ExportPart>();
                // ExportHelpers.CharacterParts(meshes, parts, asset);
                
                
                 // var parts = asset.GetOrDefault("BaseCharacterParts", Array.Empty<UObject>());
                 UObject actualAsset = getActualAsset(asset, EAssetType.Agent);

                 if (actualAsset is null)
                 {
                     throw new NullReferenceException($"Asset not found for file: {asset.GetPathName()}");
                 }
                 var parts = new List<UObject>();
                 if (actualAsset.TryGetValue(out UObject firstPersonOverlay, "MeshOverlay1P"))
                 {
                     parts.AddIfNotNull(firstPersonOverlay);
                     if (firstPersonOverlay.Properties.Count < 2)
                     {
                         actualAsset.TryGetValue(out UObject firstPersonMesh, "Mesh1P");
                         parts.AddIfNotNull(firstPersonMesh);
                     }
                 }

                 actualAsset.TryGetValue(out UObject thirdPersonMesh, "MeshCosmetic3P");
                 parts.AddIfNotNull(thirdPersonMesh);
                 parts.AddIfNotNull(GetCsMesh(asset));
                // if (asset.TryGetValue(out UObject heroDefinition, "HeroDefinition"))
                // {
                //     if (parts.Length == 0 && heroDefinition.TryGetValue(out UObject[] specializations, "Specializations"))
                //     {
                //         parts = specializations.First().GetOrDefault("CharacterParts", Array.Empty<UObject>());
                //     }
                // }

                AssetsVM.ExportChunks = parts.Count;
                foreach (var part in parts)
                {
                    var resolvedPart = Exporter.CharacterPart(part);
                    Meshes.AddIfNotNull(resolvedPart);
                    AssetsVM.ExportProgress++;
                }
                break;
            }
            case EAssetType.MachineGun:
            case EAssetType.Melee:
            case EAssetType.Rifle:
            case EAssetType.Shotgun:
            case EAssetType.Sidearm:
            case EAssetType.SMG:
            case EAssetType.SniperRifle:
                Meshes.AddRange(Exporter.WeaponDefinition(asset));
                break;
            case EAssetType.GunBuddy:
                var buddyAsset = getActualAsset(asset, EAssetType.GunBuddy);
                
                if (buddyAsset is not null && buddyAsset.TryGetValue(out UObject charm, "Charm"))
                {
                    if (charm is UStaticMesh staticMesh)
                    {
                        Meshes.AddIfNotNull(Exporter.Mesh(staticMesh));
                    }
                    else if (charm is USkeletalMesh skeletalMesh)
                    {
                        Meshes.AddIfNotNull(Exporter.Mesh(skeletalMesh));
                    }
                }

                break;
            case EAssetType.Map:
                List<UWorld> worldAssets = GetUmapsForMap(asset);
                AssetsVM.ExportChunks = worldAssets.Count;
                foreach (var worldAsset in worldAssets)
                {
                    Meshes.AddRange(Exporter.ProcessWorld(worldAsset));
                    AssetsVM.ExportProgress++;
                }
                break;
            case EAssetType.Bundle:
                Log.Information(asset.GetPathName());
                Log.Information(asset.ToString());
                break;
            case EAssetType.Mesh:
            {
                switch (asset)
                {
                    case USkeletalMesh skeletalMesh:
                        Meshes.AddIfNotNull(Exporter.Mesh(skeletalMesh));
                        break;
                    case UStaticMesh staticMesh:
                        Meshes.AddIfNotNull(Exporter.Mesh(staticMesh));
                        break;
                }

                break;
            }
            case EAssetType.World:
            {
                if (asset is not UWorld world) return;
                Meshes.AddRange(Exporter.ProcessWorld(world));
                
                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        ExportStyles(asset, styles);
    }

    private UObject? getActualAsset(UObject mainAsset, EAssetType type)
    {
        UObject actualAsset = null;
        string loadable = "None";
        switch (type)
        {
            case EAssetType.Agent:
                actualAsset = mainAsset;
                loadable = "Character";
                break;
            case EAssetType.Rifle:
                mainAsset.TryGetValue<UBlueprintGeneratedClass[]>(out var bGg, "Levels");
                actualAsset = bGg[0].ClassDefaultObject.Load();
                loadable = "None";
                break;
            case EAssetType.GunBuddy:
                mainAsset.TryGetValue<UBlueprintGeneratedClass[]>(out var bGb, "Levels");
                actualAsset = bGb[0].ClassDefaultObject.Load();
                loadable = "CharmAttachment";
                break;
        }
    
        if (actualAsset.TryGetValue(out UBlueprintGeneratedClass blueprintObject, loadable))
        {
            actualAsset = blueprintObject.ClassDefaultObject.Load();
        }

        return actualAsset;
    }

    private void ExportStyles(UObject asset, List<UObject> styles)
    {
        var metaTagsToApply = new List<FGameplayTag>();
        var metaTagsToRemove = new List<FGameplayTag>();
        foreach (var style in styles)
        {
            var tags = style.Get<FStructFallback>("MetaTags");

            var tagsToApply = tags.Get<FGameplayTagContainer>("MetaTagsToApply");
            metaTagsToApply.AddRange(tagsToApply.GameplayTags);

            var tagsToRemove = tags.Get<FGameplayTagContainer>("MetaTagsToRemove");
            metaTagsToRemove.AddRange(tagsToRemove.GameplayTags);
        }

        var metaTags = new FGameplayTagContainer(metaTagsToApply.Where(tag => !metaTagsToRemove.Contains(tag)).ToArray());
        var itemStyles = asset.GetOrDefault("ItemVariants", Array.Empty<UObject>());
        var tagDrivenStyles = itemStyles.Where(style => style.ExportType.Equals("FortCosmeticLoadoutTagDrivenVariant"));
        foreach (var tagDrivenStyle in tagDrivenStyles)
        {
            var options = tagDrivenStyle.Get<List<UObject>>("Variants");
            foreach (var option in options)
            {
                var requiredConditions = option.Get<FStructFallback[]>("RequiredConditions");
                foreach (var condition in requiredConditions)
                {
                    var metaTagQuery = condition.Get<FGameplayTagQuery>("MetaTagQuery");
                    if (metaTags.MatchesQuery(metaTagQuery)) ExportStyleData(option);
                }
            }
        }

        foreach (var style in styles) ExportStyleData(style);
    }

    private void ExportStyleData(UObject style)
    {
        var variantParts = style.GetOrDefault("VariantParts", Array.Empty<UObject>());
        foreach (var part in variantParts) OverrideMeshes.AddIfNotNull(Exporter.CharacterPart(part));

        var variantMaterials = style.GetOrDefault("VariantMaterials", Array.Empty<FStructFallback>());
        foreach (var material in variantMaterials) OverrideMaterials.AddIfNotNull(Exporter.OverrideMaterial(material));

        var variantParameters = style.GetOrDefault("VariantMaterialParams", Array.Empty<FStructFallback>());
        foreach (var parameters in variantParameters) OverrideParameters.AddIfNotNull(Exporter.OverrideParameters(parameters));
    }
    
    public UObject GetCsMesh(UObject mainAsset)
    {
        UObject csObject;
        mainAsset.TryGetValue(out csObject, "CharacterSelectFXC");
        var csExports = CUE4ParseVM.Provider.LoadAllObjects(csObject.GetPathName().Substring(0, csObject.GetPathName().LastIndexOf(".")));
        foreach (var propExp in csExports)
        {
            if (propExp.ExportType == "SkeletalMeshComponent" && propExp.Name == "SkeletalMesh_GEN_VARIABLE")
            {
                return propExp;
            }

            if (propExp.ExportType == "SCS_Node" && propExp.TryGetValue(out FPackageIndex componentTemplate, "ComponentTemplate") && componentTemplate.TryLoad(out UObject compTemplate))
            {
                Log.Information("ComponentTemplate Loaded! " + compTemplate.GetPathName());
            }
        }

        return null;
    }

    public List<UWorld> GetUmapsForMap(UObject mapAsset)
    {
        List<UWorld> worldList = new List<UWorld>();
        var mapName = mapAsset.ExportType.Replace("_PrimaryAsset_C", "");
        var mapAssetList = CUE4ParseVM.Provider.Files.Where(file => IsValidMapAsset(file.Key, mapName))
            .ToList();
        foreach (var (filePath, file) in mapAssetList)
        {
            if (CUE4ParseVM.Provider.TryLoadObject(file.PathWithoutExtension, out UWorld loadedWorld))
                worldList.AddIfNotNull(loadedWorld);
        }
        return worldList;
    }

    private bool IsValidMapAsset(string filePath, string mapName)
    {
        return filePath.EndsWith(".umap") 
               && filePath.ToLower().Contains(mapName.ToLower()) 
               && !(filePath.ToLower().Contains("kill") && filePath.ToLower().Contains("volumes"))
               && !(filePath.ToLower().Contains("graybox") && filePath.ToLower().Contains("reference")) 
               && !filePath.ToLower().Contains("gameplay.umap")
               && !filePath.ToLower().Contains("vfx.umap");
    }
}