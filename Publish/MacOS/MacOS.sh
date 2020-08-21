#!/bin/bash
APP_NAME="xDelta3 Cross GUI.app"
APP_OUTPUT_PATH="Output"
APP_TAR_NAME1="xdelta3-cross-gui_"
APP_TAR_NAME2="_macOS_x64"
PUBLISH_OUTPUT_DIRECTORY="../../bin/Release/netcoreapp3.1/publishMac/."
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

# Set version
sed 's/{VERSION}/'"$VERSION"'/g' "$APP_OUTPUT_PATH/$APP_NAME/Contents/InfoTEMP.plist" > "$APP_OUTPUT_PATH/$APP_NAME/Contents/Info.plist"
rm "$APP_OUTPUT_PATH/$APP_NAME/Contents/InfoTEMP.plist"

cp "$ICON_FILE" "$APP_OUTPUT_PATH/$APP_NAME/Contents/Resources/$ICON_FILE"
cp -a "$PUBLISH_OUTPUT_DIRECTORY" "$APP_OUTPUT_PATH/$APP_NAME/Contents/MacOS"
chmod +x "$APP_OUTPUT_PATH/$APP_NAME/Contents/MacOS/xdelta3_cross_gui"
cd "$APP_OUTPUT_PATH"
tar -czvf "$APP_TAR_NAME1$VERSION$APP_TAR_NAME2.tar.gz" "$APP_NAME/"
mv "$APP_TAR_NAME1$VERSION$APP_TAR_NAME2.tar.gz" ../../"$APP_TAR_NAME1$VERSION$APP_TAR_NAME2.tar.gz"