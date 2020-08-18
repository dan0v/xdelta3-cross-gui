# xDelta3 Cross GUI
A cross-platform GUI for xDelta3 patching, inspired by [Moodkiller/xdelta3-gui-2.0](https://github.com/Moodkiller/xdelta3-gui-2.0), now available for Windows, Linux, and MacOS.

## Features
- Compatible with Windows, Linux, and MacOS
- Scalable UI
- Add old and new files to the UI with Drag and Drop (Windows only [*upstream issue*](https://github.com/AvaloniaUI/Avalonia/issues/3502))
- Visual progress bar for patching
- View or hide terminal output during patching
- Allow for custom xDelta3 commandline arguments
- Optional zip compression of patches
- Detect and use preinstalled xDelta3 if available on System Path

## Installation
### Windows
1. Install [.Net Core 3.1 for Windows](https://docs.microsoft.com/en-us/dotnet/core/install/windows?tabs=netcore31)
2. Download latest Windows build from the [Releases page](https://github.com/dan0v/xdelta3-cross-gui/releases)
3. Run executable (`xdelta3_cross_gui.exe`)

### Linux
1. Install [.Net Core 3.1 for your distribution](https://docs.microsoft.com/en-us/dotnet/core/install/linux)
2. Download latest Linux build from the [Releases page](https://github.com/dan0v/xdelta3-cross-gui/releases)
3. Run executable (`xdelta3_cross_gui`)

### Mac
1. Install [.Net Core 3.1 for MacOS](https://docs.microsoft.com/en-us/dotnet/core/install/macos)
2. Download latest MacOS build from the [Releases page](https://github.com/dan0v/xdelta3-cross-gui/releases)
3. Run executable (`xdelta3_cross_gui`)

#### Optionally
You may provide your own builds of xDelta3, instead of using the included versions, by simply replacing the binaries in the `Assets/exec` folder with files with the same names, or including your own build in your System Path, with the name `xdelta3`
