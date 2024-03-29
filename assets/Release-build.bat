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

:append_filename
    set filenames=%filenames% "%~dp0..\bin\Release\%~1"
goto:eof

:build_zip
echo.
echo Release version: %GitNoobReleaseVersion%
echo.
echo.


del /q "Release\*" >nul 2>&1

set filenames=
rem GitNoob.exe filenames
del Release_filenames.txt >nul 2>&1
"%~dp0..\bin\Release\GitNoob.exe" "--installationFilenames=Release_filenames.txt"
for /f "tokens=* delims=" %%a in (Release_filenames.txt) do call :append_filename "%%a"
del Release_filenames.txt >nul 2>&1
rem GitNoobUpdater.exe filenames
del Release_filenames.txt >nul 2>&1
"%~dp0..\bin\Release\GitNoobUpdater.exe" "--installationFilenames=Release_filenames.txt"
for /f "tokens=* delims=" %%a in (Release_filenames.txt) do call :append_filename "%%a"
del Release_filenames.txt >nul 2>&1

"%sz_exe%" a -tzip -mx7 "Release\GitNoob-%GitNoobReleaseVersion%.zip" %filenames%
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
