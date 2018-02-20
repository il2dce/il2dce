# Define the installer name

!getdllversion "parts\IL2DCE\IL2DCE.dll" expv_
outFile "IL2DCE-${expv_1}.${expv_2}.${expv_3}.${expv_4}.exe"

# Set the default install directory
installDir "$PROGRAMFILES\Steam\steamapps\common\il-2 sturmovik cliffs of dover blitz"

Page license
Page directory
Page instfiles
UninstPage uninstConfirm
UninstPage instfiles

LicenseData "parts\IL2DCE\LICENSE"

section

# Execute uninstall
Exec $INSTDIR\uninst.exe

setOutPath "$INSTDIR\AddIns"

file "AddIns\IL2DCE.GameSingle.xml"

sectionEnd

section

setOutPath "$INSTDIR\parts\IL2DCE"

file "parts\IL2DCE\conf.ini"
file "parts\IL2DCE\LICENSE"
file "parts\IL2DCE\IL2DCE.dll"
file "parts\IL2DCE\IL2DCE.Game.dll"
file "parts\IL2DCE\IL2DCE.Mission.dll"
file "parts\IL2DCE\IL2DCE.Pages.dll"

writeUninstaller "$INSTDIR\parts\IL2DCE\uninst.exe"

sectionEnd

section

setOutPath "$INSTDIR\parts\IL2DCE\Campaigns"

file "parts\IL2DCE\Campaigns\AircraftInfo.ini"

sectionEnd

section

setOutPath "$INSTDIR\parts\IL2DCE\Campaigns\Kanalkampf"

file "parts\IL2DCE\Campaigns\Kanalkampf\CampaignInfo.ini"
file "parts\IL2DCE\Campaigns\Kanalkampf\LW_OrderOfBattle_August1940.mis"
file "parts\IL2DCE\Campaigns\Kanalkampf\MissionSingle.cs"
file "parts\IL2DCE\Campaigns\Kanalkampf\RAF_OrderOfBattle_August1940.mis"
file "parts\IL2DCE\Campaigns\Kanalkampf\StaticTemplate.mis"
file "parts\IL2DCE\Campaigns\Kanalkampf\Summer1940.mis"

sectionEnd

# Uninstall section is always called "Uninstall"
section "Uninstall"

delete "$INSTDIR\uninst.exe"

delete "$INSTDIR\..\..\AddIns\IL2DCE.GameSingle.xml"

delete "$INSTDIR\Campaigns\Kanalkampf\CampaignInfo.ini"
delete "$INSTDIR\Campaigns\Kanalkampf\LW_OrderOfBattle_August1940.mis"
delete "$INSTDIR\Campaigns\Kanalkampf\MissionSingle.cs"
delete "$INSTDIR\Campaigns\Kanalkampf\RAF_OrderOfBattle_August1940.mis"
delete "$INSTDIR\Campaigns\Kanalkampf\StaticTemplate.mis"
delete "$INSTDIR\Campaigns\Kanalkampf\Summer1940.mis"
RMDir "$INSTDIR\Campaigns\Kanalkampf"

delete "$INSTDIR\Campaigns\AircraftInfo.ini"
RMDir "$INSTDIR\Campaigns"

delete "$INSTDIR\conf.ini"
delete "$INSTDIR\LICENSE"
delete "$INSTDIR\IL2DCE.dll"
delete "$INSTDIR\IL2DCE.Game.dll"
delete "$INSTDIR\IL2DCE.Mission.dll"
delete "$INSTDIR\IL2DCE.Pages.dll"
RMDir "$INSTDIR"

sectionEnd
