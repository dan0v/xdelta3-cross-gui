![GitHub Logo](Extra%20Resources/Repository%20Cover.png)
<!-- ALL-CONTRIBUTORS-BADGE:START - Do not remove or modify this section -->
[![All Contributors](https://img.shields.io/badge/all_contributors-1-orange.svg?style=flat-square)](#contributors-)
<!-- ALL-CONTRIBUTORS-BADGE:END -->
# xDelta3 Cross GUI
A cross-platform GUI for creating xDelta3 patches, inspired by [Moodkiller/xdelta3-gui-2.0](https://github.com/Moodkiller/xdelta3-gui-2.0), now available for Windows, Linux, and MacOS. Output patches can also be applied on all three major platforms without requiring the GUI.

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
1. Install .Net Core 3.1 for MacOS [using the installer](https://docs.microsoft.com/en-us/dotnet/core/install/macos), or using Homebrew (`brew cask install dotnet`)
2. Download latest MacOS build from the [Releases page](https://github.com/dan0v/xdelta3-cross-gui/releases)
3. Run executable (`xdelta3_cross_gui`)

#### Optionally
You may provide your own builds of xDelta3, instead of using the included versions, by simply replacing the binaries in the `Assets/exec` folder with files with the same names, or including your own build in your System Path, with the name `xdelta3`

## Contributors ✨

Thanks goes to these wonderful people ([emoji key](https://allcontributors.org/docs/en/emoji-key)):

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->
<table>
  <tr>
    <td align="center"><a href="https://github.com/LeeBinder"><img src="https://avatars0.githubusercontent.com/u/39203497?v=4" width="100px;" alt=""/><br /><sub><b>LeeBinder</b></sub></a><br /><a href="#design-LeeBinder" title="Design">🎨</a></td>
  </tr>
</table>

<!-- markdownlint-enable -->
<!-- prettier-ignore-end -->
<!-- ALL-CONTRIBUTORS-LIST:END -->

This project follows the [all-contributors](https://github.com/all-contributors/all-contributors) specification. Contributions of any kind welcome!