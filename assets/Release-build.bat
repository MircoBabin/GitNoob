@echo off
setlocal
cls

set sz_exe=C:\Program Files\7-Zip\7z.exe
if exist "%sz_exe%" goto build

set sz_exe=C:\Program Files (x86)\7-Zip\7z.exe
if exist "%sz_exe%" goto build

echo 7-Zip 18.06 - 7z.exe not found
pause
goto:eof

:build 
echo 7-Zip 18.06: %sz_exe%
echo.
echo Make sure Release and Debug are batch build.
echo.
pause

echo.
echo.

cd /D "%~dp0"

del /q Release_*.zip >nul 2>&1

set files="%~dp0..\bin\Release\GitNoob.Config.dll"
set files=%files% "%~dp0..\bin\Release\GitNoob.exe"
set files=%files% "%~dp0..\bin\Release\GitNoob.exe.config"
set files=%files% "%~dp0..\bin\Release\GitNoob.Git.dll"
set files=%files% "%~dp0..\bin\Release\GitNoob.Gui.Forms.dll"
set files=%files% "%~dp0..\bin\Release\GitNoob.Gui.Forms.dll.config"
set files=%files% "%~dp0..\bin\Release\GitNoob.Gui.Program.dll"
set files=%files% "%~dp0..\bin\Release\GitNoob.ProjectTypes.dll"

"%sz_exe%" a -tzip -mx7 Release_version.zip %files%
"%sz_exe%" a -tzip -mx7 Release_version-debugpack.zip "%~dp0..\bin"

pause
