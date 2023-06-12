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

chmod +x "Assets/exec/xdelta3_x64_linux"
chmod +x "Assets/exec/xdelta3_mac"

# RUN IN POWERSHELL
#dotnet publish -r win-x86 -c Release -p:SelfContained=True -p:IncludeAllContentForSelfExtract=True -p:PublishSingleFile=True -o bin/Release/net6.0/publishWin
cd "$ORIGIN"

APP_NAME="xDelta3 Cross GUI"
APP_OUTPUT_PATH="Build"
PUBLISH_OUTPUT_DIRECTORY="../../bin/Release/net6.0/publishWin/."
APP_TAR_NAME="xdelta3-cross-gui_win_x86"

if [ -d "$APP_OUTPUT_PATH" ]
then
    rm -rf "$APP_OUTPUT_PATH"
fi

mkdir "$APP_OUTPUT_PATH"
mkdir "$APP_OUTPUT_PATH/$APP_NAME"

cp -a "$PUBLISH_OUTPUT_DIRECTORY" "$APP_OUTPUT_PATH/$APP_NAME/"
cp "../../NOTICE.txt" "$APP_OUTPUT_PATH/NOTICE.txt"
cp "../../LICENSE.txt" "$APP_OUTPUT_PATH/LICENSE.txt"

cd "$APP_OUTPUT_PATH"
zip -r "$APP_TAR_NAME.zip" "$APP_NAME/" "LICENSE.txt" "NOTICE.txt"
mv "$APP_TAR_NAME.zip" ../../"$APP_TAR_NAME.zip"