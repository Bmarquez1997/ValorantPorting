using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Objects.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValorantPorting.Services;
public class UIDataService
{
    public static async Task<UObject> GetUiDataAsync(UObject asset)
    {
        UObject uiData = null;
        if (!asset.TryGetValue(out UBlueprintGeneratedClass uiObject, "UIData"))
        {
            return uiData;
        }

        return await uiObject.ClassDefaultObject.TryLoadAsync<UObject>();
    }
    public static UObject GetUiData(UObject asset)
    {
        UObject uiData = null;
        if (!asset.TryGetValue(out UBlueprintGeneratedClass uiObject, "UIData"))
        {
            return uiData;
        }

        uiObject.ClassDefaultObject.TryLoad<UObject>(out uiData);
        return uiData;
    }
}
