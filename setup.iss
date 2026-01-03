; EFT-Loot-Tracker Inno Setup Script
#define AppName "EFT-Loot-Tracker"
#define AppVersion "1.1.1"
#define AppPublisher "Alparslan Burhan"
#define AppExeName "EFTLootTracker.exe"

[Setup]
AppId={{181DB5D0-1C60-4798-A7C2-0BBDE07BB332}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
DefaultDirName={commonappdata}\{#AppName}
DisableDirPage=yes
DefaultGroupName={#AppName}
AllowNoIcons=yes
OutputDir=.\Installer
OutputBaseFilename=EFT-Loot-Tracker-Setup
SetupIconFile=assets\EFT-Loot-Tracker.ico
LicenseFile=LICENSE.txt
Compression=lzma2/ultra64
SolidCompression=yes
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
; ProgramData requires administrative privileges for installation
PrivilegesRequired=admin

[Languages]
Name: "turkish"; MessagesFile: "compiler:Languages\Turkish.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "bin\Release\net10.0-windows\win-x64\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppExeName}"
Name: "{commondesktop}\{#AppName}"; Filename: "{app}\{#AppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#AppExeName}"; Description: "{cm:LaunchProgram,{#AppName}}"; Flags: nowait postinstall skipifsilent
