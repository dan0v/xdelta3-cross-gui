![GitHub Logo](Extra%20Resources/Repository%20Cover.png)
<!-- ALL-CONTRIBUTORS-BADGE:START - Do not remove or modify this section -->
[![All Contributors](https://img.shields.io/badge/all_contributors-6-orange.svg?style=flat-square)](#contributors-)
<!-- ALL-CONTRIBUTORS-BADGE:END -->
# xDelta3 Cross GUI
A cross-platform GUI for creating xDelta3 patches, inspired by [Moodkiller/xdelta3-gui-2.0](https://github.com/Moodkiller/xdelta3-gui-2.0), now available for Windows, Linux, and MacOS. Output patches can also be applied on all three major platforms without requiring the GUI.

## Features
- Compatible with Windows, Linux, and MacOS
- Scalable UI
- Add old and new files to the UI with Drag and Drop (Windows and MacOS only [*upstream issue*](https://github.com/AvaloniaUI/Avalonia/issues/3502))
- Visual progress bar for patching
- View or hide terminal output during patching
- Allow for custom xDelta3 commandline arguments
- Optional zip compression of patches
- Detect and use preinstalled xDelta3 if available on System Path
- Localization support ([further contributions welcome](https://github.com/dan0v/xdelta3-cross-gui/issues/12))
	- Deutsch
	- English
	- Español
	- Magyar
	- 中文(简体)
	- 日本語

## Installation
### Windows *(x86)*
#### Scoop
If you have the [Scoop](https://scoop.sh/) package manager installed, enter the following commands into Powershell to install the latest version of xDelta3 Cross GUI.

```
scoop bucket add extras
scoop install xdelta3-cross-gui
```

The application can also be updated to new versions with `scoop update xdelta3-cross-gui`. There is a few hour delay between new updates being released and being available through Scoop.

#### Manual
1. Download and unzip [latest Windows build](https://github.com/dan0v/xdelta3-cross-gui/releases/latest/download/xdelta3-cross-gui_win_x86.zip) from the [Releases page](https://github.com/dan0v/xdelta3-cross-gui/releases/)
2. Run executable (`xdelta3_cross_gui.exe`)

### Linux *(x86_64)*
1. *Optionally install [AppImageLauncher](https://github.com/TheAssassin/AppImageLauncher) to integrate xDelta3 Cross GUI into your desktop environment*
2. Download and unzip [latest Linux AppImage build](https://github.com/dan0v/xdelta3-cross-gui/releases/latest/download/xdelta3-cross-gui_linux_AppImage_x86_64.tar.gz) from the [Releases page](https://github.com/dan0v/xdelta3-cross-gui/releases/)
3. Mark `xDelta3_Cross_Gui-x86_64.AppImage` as executable (`chmod 755 xDelta3_Cross_Gui-x86_64.AppImage`)
4. Run executable (`xDelta3_Cross_Gui-x86_64.AppImage`)

### Mac *(x86_64)*
1. Download and unzip [latest MacOS build](https://github.com/dan0v/xdelta3-cross-gui/releases/latest/download/xdelta3-cross-gui_macOS_x86_64.tar.gz) from the [Releases page](https://github.com/dan0v/xdelta3-cross-gui/releases/)
2. Run (`xDelta3 Cross GUI.app`)

## Screenshots
![GitHub Logo](Extra%20Resources/Progress-demo.png)

## Contributors

All contributions are welcome! Thanks goes to these wonderful people ([emoji key](https://allcontributors.org/docs/en/emoji-key)):

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->
<table>
  <tr>
    <td align="center"><a href="https://github.com/dan0v"><img src="https://avatars1.githubusercontent.com/u/7658521?v=4?s=100" width="100px;" alt=""/><br /><sub><b>dan0v</b></sub></a><br /><a href="https://github.com/dan0v/xdelta3-cross-gui/commits?author=dan0v" title="Code">💻</a> <a href="https://github.com/dan0v/xdelta3-cross-gui/issues?q=author%3Adan0v" title="Bug reports">🐛</a> <a href="#design-dan0v" title="Design">🎨</a> <a href="https://github.com/dan0v/xdelta3-cross-gui/commits?author=dan0v" title="Documentation">📖</a> <a href="#maintenance-dan0v" title="Maintenance">🚧</a> <a href="#platform-dan0v" title="Packaging/porting to new platform">📦</a> <a href="#question-dan0v" title="Answering Questions">💬</a> <a href="https://github.com/dan0v/xdelta3-cross-gui/pulls?q=is%3Apr+reviewed-by%3Adan0v" title="Reviewed Pull Requests">👀</a> <a href="https://github.com/dan0v/xdelta3-cross-gui/commits?author=dan0v" title="Tests">⚠️</a> <a href="#userTesting-dan0v" title="User Testing">📓</a> <a href="#translation-dan0v" title="Translation">🌍</a></td>
    <td align="center"><a href="https://github.com/LeeBinder"><img src="https://avatars0.githubusercontent.com/u/39203497?v=4?s=100" width="100px;" alt=""/><br /><sub><b>LeeBinder</b></sub></a><br /><a href="#design-LeeBinder" title="Design">🎨</a></td>
    <td align="center"><img src="https://avatars0.githubusercontent.com/u/0?v=4?s=100" width="100px;" alt=""/><br /><sub><b>R.</b></sub><br /><a href="#translation" title="Translation">🌍</a></td>
    <td align="center"><a href="https://github.com/Hambaka"><img src="https://avatars.githubusercontent.com/u/20476755?v=4?s=100" width="100px;" alt=""/><br /><sub><b>Hambaka</b></sub></a><br /><a href="#translation-Hambaka" title="Translation">🌍</a></td>
    <td align="center"><a href="https://github.com/1024mb"><img src="https://avatars.githubusercontent.com/u/9301204?v=4?s=100" width="100px;" alt=""/><br /><sub><b>1024mb</b></sub></a><br /><a href="#translation-1024mb" title="Translation">🌍</a></td>
    <td align="center"><a href="https://bandism.net/"><img src="https://avatars.githubusercontent.com/u/22633385?v=4?s=100" width="100px;" alt=""/><br /><sub><b>Ikko Ashimine</b></sub></a><br /><a href="#translation-eltociear" title="Translation">🌍</a></td>
  </tr>
</table>

<!-- markdownlint-restore -->
<!-- prettier-ignore-end -->

<!-- ALL-CONTRIBUTORS-LIST:END -->

This project follows the [all-contributors](https://github.com/all-contributors/all-contributors) specification.
