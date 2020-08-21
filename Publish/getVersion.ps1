param(
    [string]$TargetDir
)

$Version = (Get-Command ${TargetDir}xdelta3_cross_gui.exe).FileVersionInfo.FileVersion