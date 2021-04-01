#!/bin/bash

echo 'Copyright 2020-2021 dan0v

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
#dotnet publish --configuration Release --framework netcoreapp3.1 -r linux-x64 --self-contained true /p:PublishTrimmed=true -o bin/Release/netcoreapp3.1/publishLinux
dotnet publish -r linux-x64 -c Release -o bin/Release/net5.0/publishLinux
cd "$ORIGIN"

APP_NAME="xDelta3 Cross GUI"
APP_OUTPUT_PATH="Build"
PUBLISH_OUTPUT_DIRECTORY="../../bin/Release/net5.0/publishLinux/."
APP_TAR_NAME1="xdelta3-cross-gui_"
APP_TAR_NAME2="_linux_AppImage_x86_64"

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

VERSION=$(cat ../version.txt | sed 's/ *$//g' | sed 's/\r//' | sed ':a;N;$!ba;s/\n//g')

cd "$APP_OUTPUT_PATH"

../Sources/appimagetool-x86_64.AppImage "$APP_NAME"

tar -czvf "$APP_TAR_NAME1$VERSION$APP_TAR_NAME2.tar.gz" "xDelta3_Cross_Gui-x86_64.AppImage" "LICENSE.txt" "NOTICE.txt"
mv "$APP_TAR_NAME1$VERSION$APP_TAR_NAME2.tar.gz" ../../"$APP_TAR_NAME1$VERSION$APP_TAR_NAME2.tar.gz"