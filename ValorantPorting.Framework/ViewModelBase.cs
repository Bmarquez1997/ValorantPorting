using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ValorantPorting.Framework;

public class ViewModelBase : ObservableObject
{
    public virtual async Task Initialize()
    {
    }
}