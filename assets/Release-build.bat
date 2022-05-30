@echo off
setlocal
cls

cd /D "%~dp0"


set sz_exe=C:\Program Files\7-Zip\7z.exe
if exist "%sz_exe%" goto build

set sz_exe=C:\Program Files (x86)\7-Zip\7z.exe
if exist "%sz_exe%" goto build

echo !!! 7-Zip 18.06 - 7z.exe not found
pause
goto:eof

:build 
echo 7-Zip 18.06: %sz_exe%

del Release_version.txt >nul 2>&1

"%~dp0..\bin\Release\GitNoob.exe" "--version=Release_version.txt"
set /p GitNoobReleaseVersion=< Release_version.txt
del Release_version.txt >nul 2>&1

"%~dp0..\bin\Debug\GitNoob.exe" "--version=Release_version.txt"
set /p GitNoobDebugVersion=< Release_version.txt
del Release_version.txt >nul 2>&1

if "%GitNoobReleaseVersion%" == "%GitNoobDebugVersion%" goto build_zip
echo.
echo Release version: %GitNoobReleaseVersion%
echo Debug version..: %GitNoobDebugVersion%
echo.
echo !!! Versions do not match.
pause
goto :eof


:build_zip
echo.
echo Release version: %GitNoobReleaseVersion%
echo.
echo.


del /q "Release\*" >nul 2>&1

set files="%~dp0..\bin\Release\GitNoob.Config.dll"
set files=%files% "%~dp0..\bin\Release\GitNoob.exe"
set files=%files% "%~dp0..\bin\Release\GitNoob.exe.config"
set files=%files% "%~dp0..\bin\Release\GitNoob.Git.dll"
set files=%files% "%~dp0..\bin\Release\GitNoob.Gui.Forms.dll"
set files=%files% "%~dp0..\bin\Release\GitNoob.Gui.Forms.dll.config"
set files=%files% "%~dp0..\bin\Release\GitNoob.Gui.Program.dll"
set files=%files% "%~dp0..\bin\Release\GitNoob.ProjectTypes.dll"

"%sz_exe%" a -tzip -mx7 "Release\GitNoob-%GitNoobReleaseVersion%.zip" %files%
"%sz_exe%" a -tzip -mx7 "Release\GitNoob-%GitNoobReleaseVersion%-debugpack.zip" "%~dp0..\bin"

echo.
echo.
echo Created "Release\GitNoob-%GitNoobReleaseVersion%.zip"
echo Created "Release\GitNoob-%GitNoobReleaseVersion%-debugpack.zip" 

rem https://github.com/MircoBabin/GitNoob/releases/latest/download/release.download.zip.url-location
rem Don't output trailing newline (CRLF)
<NUL >"Release\release.download.zip.url-location" set /p="https://github.com/MircoBabin/GitNoob/releases/download/%GitNoobReleaseVersion%/GitNoob-%GitNoobReleaseVersion%.zip"

echo.
echo Created "Release\release.download.zip.url-location" 
echo.

pause
