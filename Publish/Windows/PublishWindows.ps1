cd -Path "$(Get-Location)\..\..\"
dotnet publish -r win-x86 -c Release -p:SelfContained=True -p:IncludeAllContentForSelfExtract=True -p:PublishSingleFile=True -o bin/Release/net6.0/publishWin
