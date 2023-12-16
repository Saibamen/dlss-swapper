﻿!include "MUI2.nsh"
!include "LogicLib.nsh"
!include "StrContains.nsh"

; define name of installer
OutFile "installer.exe"

!define UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\DLSS Swapper"

!define UninstLog "uninstall.log"
Var UninstLog

Function .onInit
  ; Set defualt install location
  StrCpy $INSTDIR "$PROGRAMFILES64\DLSS Swapper\"
  ClearErrors
  ReadRegStr $0 SHCTX "${UNINST_KEY}" "InstallLocation"
  ${If} ${Errors}
    ; No-op
  ${Else}
    StrCpy $INSTDIR "$0\"
  ${EndIf}
FunctionEnd


; Install directory should have "dlss" in it, if not we should add it. 
; See issue #169 for what the consiquenses are if a user selects a directory
; to install to which already contains other files.
Function .onVerifyInstDir
  ${StrContains} $0 "dlss" $INSTDIR
  StrCmp $0 "" badPath
    Goto done
  badPath:
    StrCpy $INSTDIR "$INSTDIR\DLSS Swapper\"
  done:
FunctionEnd


Function OnInstFilesPre
  ; If the install directory does not contain "dlss" in it we should
  ; probably add it to keep the user safe. See issue #169 as to why
  ; this is useful.
  ${StrContains} $0 "dlss" $INSTDIR
  StrCmp $0 "" badPath
    Goto done
  badPath:
    StrCpy $INSTDIR "$INSTDIR\DLSS Swapper\"
    MessageBox MB_OK "Install path updated to $INSTDIR"
  done:
FunctionEnd


; Used to launch DLSS Swapper after install is complete.
Function LaunchLink
  ExecShell "" "$SMPROGRAMS\DLSS Swapper.lnk"
FunctionEnd


; For removing Start Menu shortcut in Windows 7
; RequestExecutionLevel user
RequestExecutionLevel highest


; App version information
Name "DLSS Swapper"
!define MUI_ICON "..\..\src\Assets\icon.ico"
!define MUI_VERSION "1.0.2.0"
!define MUI_PRODUCT "DLSS Swapper"
VIProductVersion "1.0.2.0"
VIAddVersionKey "ProductName" "DLSS Swapper"
VIAddVersionKey "ProductVersion" "1.0.2.0"
VIAddVersionKey "FileDescription" "DLSS Swapper installer"
VIAddVersionKey "FileVersion" "1.0.2.0"


; Pages
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_DIRECTORY
!define MUI_PAGE_CUSTOMFUNCTION_PRE OnInstFilesPre
!insertmacro MUI_PAGE_INSTFILES
 

; These indented statements modify settings for MUI_PAGE_FINISH
!define MUI_FINISHPAGE_NOAUTOCLOSE
!define MUI_FINISHPAGE_RUN
!define MUI_FINISHPAGE_RUN_CHECKED
!define MUI_FINISHPAGE_RUN_TEXT "Launch now"
!define MUI_FINISHPAGE_RUN_FUNCTION "LaunchLink"
!insertmacro MUI_PAGE_FINISH


; Uninstaller pages
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES


; Languages
!insertmacro MUI_LANGUAGE "English"


!macro CreateDirectoryToInstaller Path
  CreateDirectory "$INSTDIR\${Path}"
  FileWrite $UninstLog "${Path}$\r$\n"
!macroend


!macro AddFileToInstaller FileName FullFileName
  FileWrite $UninstLog "${FileName}$\r$\n"
  File "/oname=${FileName}" "${FullFileName}"
!macroend


Section -openlogfile
  CreateDirectory "$INSTDIR"
  IfFileExists "$INSTDIR\${UninstLog}" +3
    FileOpen $UninstLog "$INSTDIR\${UninstLog}" w
  Goto +4
    SetFileAttributes "$INSTDIR\${UninstLog}" NORMAL
    FileOpen $UninstLog "$INSTDIR\${UninstLog}" a
    FileSeek $UninstLog 0 END
SectionEnd

 
; start default section
Section
  ; set the installation directory as the destination for the following actions
  SetOutPath $INSTDIR

  ; Adds files from list that was auto-generated by build_Installer.ps1
  !include "FileList.nsh"
  ;${File} "..\..\src\bin\publish\unpackaged\*"
  ;!tempfile filelist
  ;!system 'FOR /R "..\..\src\bin\publish\unpackaged\" %A IN (*.*) DO @( >> "${filelist}" echo."${File\} \"%~A\"" )'
  ;!delfile "${filelist}"
  ;!inc
  
  ; create the uninstaller
  WriteUninstaller "$INSTDIR\uninstall.exe"
  FileWrite $UninstLog "uninstall.exe$\r$\n"

  # create a shortcut named "new shortcut" in the start menu programs directory
  # point the new shortcut at the program uninstaller
  CreateShortcut "$SMPROGRAMS\DLSS Swapper.lnk" "$INSTDIR\DLSS Swapper.exe"

  WriteRegStr SHCTX "${UNINST_KEY}" "DisplayName" "DLSS Swapper"
  WriteRegStr SHCTX "${UNINST_KEY}" "DisplayVersion" "1.0.2.0"
  WriteRegStr SHCTX "${UNINST_KEY}" "Publisher" "beeradmoore"
  WriteRegStr SHCTX "${UNINST_KEY}" "DisplayIcon" "$\"$INSTDIR\DLSS Swapper.exe$\""
  WriteRegStr SHCTX "${UNINST_KEY}" "UninstallString" "$\"$INSTDIR\uninstall.exe$\""
  WriteRegStr SHCTX "${UNINST_KEY}" "InstallLocation" $INSTDIR
SectionEnd


; Close the log file off and set it as a readonly hidden system file.
Section -closelogfile
  FileClose $UninstLog
  SetFileAttributes "$INSTDIR\${UninstLog}" READONLY|SYSTEM|HIDDEN
SectionEnd


; uninstaller section start
Section "Uninstall"
  #$LOCALAPPDATA
  #$APPDATAS

  ;Can't uninstall if uninstall log is missing!
  IfFileExists "$INSTDIR\${UninstLog}" +3
    MessageBox MB_OK|MB_ICONSTOP "${UninstLog} not found.$\r$\nUninstallation cannot proceed."
      Abort
 
  Push $R0
  Push $R1
  Push $R2
  SetFileAttributes "$INSTDIR\${UninstLog}" NORMAL
  FileOpen $UninstLog "$INSTDIR\${UninstLog}" r
  StrCpy $R1 -1
 
  GetLineCount:
    ClearErrors
    FileRead $UninstLog $R0
    IntOp $R1 $R1 + 1
    StrCpy $R0 $R0 -2
    Push $R0   
    IfErrors 0 GetLineCount
 
  Pop $R0
 
  LoopRead:
    StrCmp $R1 0 LoopDone
    Pop $R0
 
    IfFileExists "$INSTDIR\$R0\*.*" 0 +3
      RMDir "$INSTDIR\$R0"  #is dir
    Goto +3
    IfFileExists "$INSTDIR\$R0" 0 +2
      Delete "$INSTDIR\$R0" #is file

    IntOp $R1 $R1 - 1
    Goto LoopRead
  LoopDone:
  FileClose $UninstLog
  Delete "$INSTDIR\${UninstLog}"
  RMDir "$INSTDIR"
  Pop $R2
  Pop $R1
  Pop $R0
  
  ; Remove registry keys
  DeleteRegKey SHCTX "${UNINST_KEY}"

  ; Remove start menu shortcut.
  Delete "$SMPROGRAMS\DLSS Swapper.lnk"

SectionEnd
