; EFT-Loot-Tracker Inno Setup Script
; Versiyon 1.1.1
; Tarih: 3 OCAK 2026

#define MyAppName "EFT-Loot-Tracker"
#define MyAppVersion "1.1.1"
#define MyAppPublisher "Alparslan Burhan"
#define MyAppExeName "EFTLootTracker.exe"
#define MyAppYear "2026"

[Setup]
AppId={{181DB5D0-1C60-4798-A7C2-0BBDE07BB332}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppCopyright=Telif Hakkı © {#MyAppYear} - Tüm hakları saklıdır
VersionInfoVersion={#MyAppVersion}

; KURULUM AYARLARI
DefaultDirName={commonappdata}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=no
LicenseFile=LICENSE.txt
OutputDir=Installer
OutputBaseFilename=EFT-Loot-Tracker-Setup-v{#MyAppVersion}
SetupIconFile=assets\EFT-Loot-Tracker.ico
UninstallDisplayIcon={app}\{#MyAppExeName}
ChangesAssociations=no

; SIKIŞTIRMA AYARLARI
Compression=lzma2/ultra64
SolidCompression=yes
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64

; GÜVENLİK VE YETKİLENDİRME
PrivilegesRequired=admin
PrivilegesRequiredOverridesAllowed=dialog

; GÖRÜNÜM AYARLARI
WizardStyle=modern
DisableWelcomePage=no
ShowLanguageDialog=auto
WindowVisible=no

; KALDIRMA AYARLARI
Uninstallable=yes
UninstallDisplayName={#MyAppName}
UninstallFilesDir={app}\Uninstall

[Languages]
Name: "turkish"; MessagesFile: "compiler:Languages\Turkish.isl"

[Tasks]
Name: "desktopicon"; Description: "Masaüstüne kısayol ekle"; GroupDescription: "Kısayollar:"; Flags: unchecked
Name: "startmenu"; Description: "Başlat Menüsüne kısayol ekle"; GroupDescription: "Kısayollar:"; Flags: checkedonce

[Files]
; ANA UYGULAMA DOSYALARI - DİL KLASÖRLER HARİÇ
Source: "bin\Release\net10.0-windows\win-x64\publish\*"; DestDir: "{app}"; Flags: ignoreversion; Excludes: "*.pdb,ar-*,am-*,az-*,be-*,bg-*,bn-*,bs-*,ca-*,cs-*,da-*,de-*,el-*,en-*,es-*,et-*,eu-*,fa-*,fi-*,fil-*,fr-*,gl-*,he-*,hi-*,hr-*,hu-*,id-*,is-*,it-*,ja-*,ka-*,kk-*,km-*,kn-*,ko-*,ky-*,lo-*,lt-*,lv-*,mk-*,ml-*,mn-*,mr-*,ms-*,nb-*,ne-*,nl-*,pa-*,pl-*,pt-*,ro-*,ru-*,si-*,sk-*,sl-*,sq-*,sr-*,sv-*,sw-*,ta-*,te-*,th-*,ti-*,tk-*,uk-*,ur-*,uz-*,vi-*,zh-*,af-*"

; Assets klasörünü kopyala (Kök dizinden)
Source: "assets\*"; DestDir: "{app}\assets"; Flags: ignoreversion recursesubdirs createallsubdirs

Source: "LICENSE.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "README.md"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; WorkingDir: "{app}"; IconFilename: "{app}\assets\EFT-Loot-Tracker.ico"; Comment: "Escape from Tarkov Loot Tracker"; Tasks: desktopicon
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; WorkingDir: "{app}"; IconFilename: "{app}\assets\EFT-Loot-Tracker.ico"; Comment: "Escape from Tarkov Loot Tracker"; Tasks: startmenu
Name: "{autoprograms}\{#MyAppName}\{#MyAppName} - Kaldır"; Filename: "{uninstallexe}"; IconFilename: "{app}\assets\EFT-Loot-Tracker.ico"

[UninstallDelete]
; KURULUM SONRASI OLUŞAN TÜM VERİLERİ TEMİZLE
Type: filesandordirs; Name: "{app}\data"
Type: filesandordirs; Name: "{app}\cache"
Type: files; Name: "{app}\settings.ini"
Type: dirifempty; Name: "{app}"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{#MyAppName} uygulamasını şimdi çalıştır"; Flags: nowait postinstall skipifsilent

[Messages]
; TÜRKÇE ÖZEL MESAJLAR
WelcomeLabel2=Bu sihirbaz, [name/ver] uygulamasını bilgisayarınıza kuracak.%n%nKuruluma devam etmeden önce diğer tüm uygulamaları kapatmanız önerilir.
FinishedHeadingLabel=Kurulum Tamamlandı!
FinishedLabel=[name] başarıyla kuruldu.%n%nUygulamayı çalıştırdığınızda Tarkov Wiki verileri otomatik olarak senkronize edilecektir.
