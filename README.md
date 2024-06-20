# ValorantPorting 
A free and open-source tool created to automate the Valorant porting process to Blender and Unreal Engine
------------------------------------------

#### Powered by [Avalonia UI](https://avaloniaui.net/) and [CUE4Parse](https://github.com/FabianFG/CUE4Parse)

[![Discord](https://discord.com/api/guilds/866821077769781249/widget.png?style=shield)](https://discord.com/invite/valorant3d)
[![Blender](https://img.shields.io/badge/Blender-4.0+-blue?logo=blender&logoColor=white&color=orange)](https://www.blender.org/download/)
[![Release](https://img.shields.io/github/release/Ka1serM/ValorantPorting)]()
[![Downloads](https://img.shields.io/github/downloads/Ka1serM/ValorantPorting/total?color=green)]()
***

# Installation

### Requirements
* [Blender 4.0 or higher](https://www.blender.org/download/)
* [Visual C++ Distributables x64](https://docs.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist?view=msvc-170)
* [.NET 8.0 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0/runtime)
> ⚠️ ValorantPorting requires **.NET 8.0 or later** to work, download it from the link above and select the **Windows Desktop x64** version.

## ValorantPorting Client
* Download `ValorantPorting.exe` from the [latest release](https://github.com/KaiserM21/ValorantPorting/releases) to a location where programs have read/write permissions (Avoid Downloads/Desktop)
* Launch the `ValorantPorting.exe` executable

## Blender Plugin
* To install the Blender plugin, add your Blender installation in the Plugin tab and press "sync".
* NOTE: This needs to be your actual blender.exe, not a desktop/start menu shortcut.  By default, this can be found at
  
  ```C:\Program Files\Blender Foundation\<blender.version>\blender.exe```

## Building ValorantPorting

To build ValorantPorting from source, first clone the repository and all of its submodules.

```
git clone -b v2 https://github.com/Ka1serM/ValorantPorting --recursive
```

Then run BuildRelease.bat or open the project directory in a terminal window and publish

```
dotnet publish ValorantPorting -c Release --no-self-contained -r win-x64 -o "./Release" -p:PublishSingleFile=true -p:DebugType=None -p:DebugSymbols=false -p:IncludeNativeLibrariesForSelfExtract=true
```

### Credits:
* Valorant [live] Code: https://github.com/4sval/FModel & https://github.com/FortniteCentral/MercuryCommons 
* https://github.com/halfuwu
* https://github.com/djhaled
* https://github.com/KaiserM21
* https://github.com/Bmarquez1997
