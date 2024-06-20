using System;
using System.Collections.Generic;
using System.Linq;
using ATL.Logging;
using Avalonia.Controls;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Objects.Core.i18N;
using CUE4Parse.UE4.Objects.Engine;
using Serilog;
using ValorantPorting.Extensions;
using ValorantPorting.Services;

namespace ValorantPorting.Controls.Assets;

public partial class AssetOptions : UserControl
{
    public AssetItem AssetItem { get; set; }

    public AssetOptions(AssetItem assetItem)
    {
        InitializeComponent();

        AssetItem = assetItem;

        Styles.Items.Clear();
        var styles = AssetItem.Asset.GetOrDefault("Chromas", Array.Empty<UObject>());
        var options = new List<UObject>();
        foreach (UBlueprintGeneratedClass style in styles)
        {
            // TODO: Fix styles
            //if (style == null)
            //{
            //    continue;
            //}
            //var CDO = style.ClassDefaultObject.Load();
            //UObject uiAsset = UIDataService.GetUiData(CDO);
            //options.Add(uiAsset);
        }

            //var styleSelector = new StyleItem("Chromas", options, AssetItem.IconBitmap);
            //if (styleSelector.Styles.Count == 0) return;
            //Styles.Items.Add(styleSelector);
    }

    public List<UObject> GetSelectedStyles()
    {
        return Styles.Items
            .Cast<StyleItem>()
            .Select(x => (StyleEntry) x.StylesListBox.SelectedItem!)
            .RemoveNull()
            .Select(x => x.StyleInfo)
            .ToList();
    }
}