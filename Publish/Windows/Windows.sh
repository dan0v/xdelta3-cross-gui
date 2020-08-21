#!/bin/bash
cd "$(dirname "$0")"
APP_NAME="xDelta3 Cross GUI"
APP_OUTPUT_PATH="Build"
PUBLISH_OUTPUT_DIRECTORY="../../bin/Release/netcoreapp3.1/publishWin/."
APP_TAR_NAME1="xdelta3-cross-gui_"
APP_TAR_NAME2="_win_x86_x64"

if [ -d "$APP_OUTPUT_PATH" ]
then
    rm -rf "$APP_OUTPUT_PATH"
fi

mkdir "$APP_OUTPUT_PATH"
mkdir "$APP_OUTPUT_PATH/$APP_NAME"

cp -a "$PUBLISH_OUTPUT_DIRECTORY" "$APP_OUTPUT_PATH/$APP_NAME/"

VERSION=$(cat ../version.txt | sed 's/ *$//g' | sed 's/\r//' | sed ':a;N;$!ba;s/\n//g')

cd "$APP_OUTPUT_PATH"
zip -r "$APP_TAR_NAME1$VERSION$APP_TAR_NAME2.zip" "$APP_NAME/"
mv "$APP_TAR_NAME1$VERSION$APP_TAR_NAME2.zip" ../../"$APP_TAR_NAME1$VERSION$APP_TAR_NAME2.zip"