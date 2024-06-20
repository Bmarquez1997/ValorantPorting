using ValorantPorting.Export.Types;
using ValorantPorting.ViewModels;

namespace ValorantPorting.Export;

public class ExportResponse
{
    public string AssetsFolder;
    public ExportOptionsBase Options;
    public ExportDataBase[] Data;
}