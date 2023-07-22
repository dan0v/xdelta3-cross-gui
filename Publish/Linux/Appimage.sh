#!/bin/bash

echo 'Copyright 2020-2023 dan0v

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
'

ORIGIN="$(pwd)"
cd "../.."
dotnet publish -r linux-x64 -c Release -p:SelfContained=True -o bin/Release/net7.0/publishLinux

cd "$ORIGIN"
cd "Sources"
unzip -o appimagetool-x86_64.AppImage.zip
chmod +x appimagetool-x86_64.AppImage
cd "$ORIGIN"

chmod +x Sources/appimagetool-x86_64.AppImage

APP_NAME="xDelta3 Cross GUI"
APP_OUTPUT_PATH="Build"
PUBLISH_OUTPUT_DIRECTORY="../../bin/Release/net7.0/publishLinux/."
APP_TAR_NAME="xdelta3-cross-gui_linux_AppImage_x86_64"

chmod +x "$PUBLISH_OUTPUT_DIRECTORY/Assets/exec/xdelta3_x64_linux"
chmod +x "$PUBLISH_OUTPUT_DIRECTORY/Assets/exec/xdelta3_mac"

if [ -d "$APP_OUTPUT_PATH" ]
then
    rm -rf "$APP_OUTPUT_PATH"
fi

mkdir -p "$APP_OUTPUT_PATH/$APP_NAME/usr/bin"
mkdir -p "$APP_OUTPUT_PATH/$APP_NAME/usr/share/metainfo"

cp -a "$PUBLISH_OUTPUT_DIRECTORY" "$APP_OUTPUT_PATH/$APP_NAME/usr/bin/"
cp -a "Sources/AppRun" "$APP_OUTPUT_PATH/$APP_NAME/AppRun"
cp -a "Sources/xdelta3_cross_gui.desktop" "$APP_OUTPUT_PATH/$APP_NAME/xdelta3_cross_gui.desktop"
cp -a "Sources/xdelta3_cross_gui.appdata.xml" "$APP_OUTPUT_PATH/$APP_NAME/usr/share/metainfo/xdelta3_cross_gui.appdata.xml"
cp -a "Sources/icn.png" "$APP_OUTPUT_PATH/$APP_NAME/icn.png"

chmod 755 "$APP_OUTPUT_PATH/$APP_NAME/AppRun"

cp "../../NOTICE.txt" "$APP_OUTPUT_PATH/NOTICE.txt"
cp "../../LICENSE.txt" "$APP_OUTPUT_PATH/LICENSE.txt"

cd "$APP_OUTPUT_PATH"

ARCH=x86_64 ../Sources/appimagetool-x86_64.AppImage -n "$APP_NAME"

tar -czvf "$APP_TAR_NAME.tar.gz" "xDelta3_Cross_Gui-x86_64.AppImage" "LICENSE.txt" "NOTICE.txt"
mv "$APP_TAR_NAME.tar.gz" ../../"$APP_TAR_NAME.tar.gz"