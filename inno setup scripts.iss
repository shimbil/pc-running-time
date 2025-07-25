; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "PC Running Time"
#define MyAppVersion "1.0"
#define MyAppPublisher "Al Shimbil Khan"
#define MyAppURL "http://facebook.com/shimbilmax"
#define MyAppExeName "PCRunningTime.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{3AE34D39-FBCA-4D07-8FB5-46408FF20EBC}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DisableProgramGroupPage=yes
OutputBaseFilename=setup_pc_running_time_v1.0
SetupIconFile=D:\WORKS\SHIMBIL\PCRunningTime\bin\Release\app_icon.ico
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "D:\WORKS\SHIMBIL\PCRunningTime\bin\Release\PCRunningTime.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\WORKS\SHIMBIL\PCRunningTime\bin\Release\app_icon.ico"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\WORKS\SHIMBIL\PCRunningTime\bin\Release\PCRunningTime.exe.config"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{commonprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

