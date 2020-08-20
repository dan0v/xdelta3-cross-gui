#!/bin/bash
APP_NAME="xDelta3 Cross GUI.app"
APP_OUTPUT_PATH="Output"
APP_TAR_NAME="xdelta3-cross-gui_XXX_macOS_x64"
PUBLISH_OUTPUT_DIRECTORY="../../bin/Release/netcoreapp3.1/publish/."
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

cp "$INFO_PLIST" "$APP_OUTPUT_PATH/$APP_NAME/Contents/Info.plist"
cp "$ICON_FILE" "$APP_OUTPUT_PATH/$APP_NAME/Contents/Resources/$ICON_FILE"
cp -a "$PUBLISH_OUTPUT_DIRECTORY" "$APP_OUTPUT_PATH/$APP_NAME/Contents/MacOS"
chmod +x "$APP_OUTPUT_PATH/$APP_NAME/Contents/MacOS/xdelta3_cross_gui"
cd "$APP_OUTPUT_PATH"
tar -czvf "$APP_TAR_NAME.tar.gz" "$APP_NAME"