@echo off
   if not exist c:\incoming md c:\incoming
   if not exist c:\incoming goto :eof
   
   echo [STEP] Removing c:\incoming\general-project
   rd /s /q c:\incoming\general-project
   md c:\incoming\general-project
   
   echo [STEP] Copy to c:\incoming\general-project
   xcopy /s /e "%~dp0" c:\incoming\general-project
   
   echo [STEP] Initialize git
   cd /d c:\incoming\general-project\src
   git init
   git add README.md
   git commit -m "a commit"
   
   git config user.name "Mirco Babin"
   git config user.email "mirco@..."
   
   echo [STEP] Start GitNoob
   start "" "%~dp0\..\..\bin\release\gitnoob.exe" "c:\incoming\general-project\GitNoob\GitNoob.ini"
   
   pause