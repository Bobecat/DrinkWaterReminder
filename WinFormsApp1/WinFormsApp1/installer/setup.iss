[Setup]
AppName=喝水提醒
AppVersion=1.0
DefaultDirName={pf64}\DrinkWaterReminder
DefaultGroupName=DrinkWaterReminder
OutputBaseFilename=DrinkWaterInstaller
Compression=lzma
SolidCompression=yes

[Files]
Source: "E:\WinFormsApp1\WinFormsApp1\WinFormsApp1\bin\Release\net10.0-windows\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\喝水提醒"; Filename: "{app}\WinFormsApp1.exe"
Name: "{commondesktop}\喝水提醒"; Filename: "{app}\WinFormsApp1.exe"

[Run]
Filename: "{app}\WinFormsApp1.exe"; Description: "启动 喝水提醒"; Flags: nowait postinstall skipifsilent

; Note: Replace {#PUBLISH_DIR} when packaging the installer. Use the publish.ps1 script to create publish output.
