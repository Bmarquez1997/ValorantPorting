namespace ValorantPorting.Framework.ViewModels.Endpoints.Models;

public class ReleaseResponse
{
    public string Version;
    public VPVersion ProperVersion => new(Version);
    
    public string DownloadUrl;
    public string ChangelogUrl;
    public bool IsMandatory;

    public DependencyResponse[] Dependencies;
}

public class DependencyResponse
{
    public string Name;
    public string URL;
}

public class VPVersion
{
    public readonly int Major;
    public readonly int Minor;
    public readonly int Patch;

    public VPVersion(string inVersion)
    {
        var mainVersioning = inVersion.Split(".");
        Major = int.Parse(mainVersioning[0]);
        Minor = int.Parse(mainVersioning[1]);
        Patch = int.Parse(mainVersioning[2]);
    }

    public VPVersion(int major = 2, int minor = 0, int patch = 0, string subversion = "")
    {
        Major = major;
        Minor = minor;
        Patch = patch;
    }

    public bool MajorEquals(VPVersion other)
    {
        return Major == other.Major;
    }
    
    public bool MinorEquals(VPVersion other)
    {
        return Minor == other.Minor;
    }
    
    public bool PatchEquals(VPVersion other)
    {
        return Patch == other.Patch;
    }

    public override string ToString()
    {
        return $"{Major}.{Minor}.{Patch}";
    }

    public override bool Equals(object? obj)
    {
        var other = (VPVersion) obj!;
        return MajorEquals(other) && MinorEquals(other) && PatchEquals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Major, Minor, Patch);
    }
}