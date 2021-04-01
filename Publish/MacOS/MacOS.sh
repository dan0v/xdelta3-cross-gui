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
dotnet publish -r osx-x64 -c Release -p:SelfContained=True -o bin/Release/net5.0/publishMac
cd "$ORIGIN"

APP_NAME="xDelta3 Cross GUI.app"
APP_OUTPUT_PATH="Output"
APP_TAR_NAME1="xdelta3-cross-gui_"
APP_TAR_NAME2="_macOS_x86_64"
PUBLISH_OUTPUT_DIRECTORY="../../bin/Release/net5.0/publishMac/."
INFO_PLIST="Info.plist"
ICON_FILE="Icon.icns"

if [ -d "$APP_OUTPUT_PATH" ]
then
    rm -rf "$APP_OUTPUT_PATH"
fi

mkdir "$APP_OUTPUT_PATH"
mkdir "$APP_OUTPUT_PATH/$APP_NAME"

mkdir "$APP_OUTPUT_PATH/$APP_NAME/Contents"
mkdir "$APP_OUTPUT_PATH/$APP_NAME/Contents/MacOS"
mkdir "$APP_OUTPUT_PATH/$APP_NAME/Contents/Resources"

VERSION=$(cat ../version.txt | sed 's/ *$//g' | sed 's/\r//' | sed ':a;N;$!ba;s/\n//g')

cp "$INFO_PLIST" "$APP_OUTPUT_PATH/$APP_NAME/Contents/InfoTEMP.plist"

cp "../../NOTICE.txt" "$APP_OUTPUT_PATH/NOTICE.txt"
cp "../../LICENSE.txt" "$APP_OUTPUT_PATH/LICENSE.txt"

# Set version
sed 's/{VERSION}/'"$VERSION"'/g' "$APP_OUTPUT_PATH/$APP_NAME/Contents/InfoTEMP.plist" > "$APP_OUTPUT_PATH/$APP_NAME/Contents/Info.plist"
rm "$APP_OUTPUT_PATH/$APP_NAME/Contents/InfoTEMP.plist"

cp "$ICON_FILE" "$APP_OUTPUT_PATH/$APP_NAME/Contents/Resources/$ICON_FILE"
cp -a "$PUBLISH_OUTPUT_DIRECTORY" "$APP_OUTPUT_PATH/$APP_NAME/Contents/MacOS"
chmod +x "$APP_OUTPUT_PATH/$APP_NAME/Contents/MacOS/xdelta3_cross_gui"
cd "$APP_OUTPUT_PATH"
tar -czvf "$APP_TAR_NAME1$VERSION$APP_TAR_NAME2.tar.gz" "$APP_NAME/" "LICENSE.txt" "NOTICE.txt"
mv "$APP_TAR_NAME1$VERSION$APP_TAR_NAME2.tar.gz" ../../"$APP_TAR_NAME1$VERSION$APP_TAR_NAME2.tar.gz"