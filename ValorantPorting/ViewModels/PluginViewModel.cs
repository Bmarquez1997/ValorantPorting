using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using ValorantPorting.Framework;

namespace ValorantPorting.ViewModels;

public partial class PluginViewModel : ViewModelBase
{
    [ObservableProperty] private BlenderPluginViewModel blender = new();
    //[ObservableProperty] private UnrealPluginViewModel unreal = new();

    public override async Task Initialize()
    {
        if (Blender.AutomaticUpdate)
        {
            await Blender.SyncAll(true);
        }
        //if (Unreal.AutomaticUpdate)
        //{
        //    await Unreal.SyncAll(true);
        //}
    }
}

