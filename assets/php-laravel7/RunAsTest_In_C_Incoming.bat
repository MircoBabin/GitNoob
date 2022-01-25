@echo off
   if not exist c:\incoming md c:\incoming
   if not exist c:\incoming goto :eof
   
   echo [STEP] Removing c:\incoming\php-laravel7
   rd /s /q c:\incoming\php-laravel7
   md c:\incoming\php-laravel7
   
   echo [STEP] Copy to c:\incoming\php-laravel7
   xcopy /s /e "%~dp0" c:\incoming\php-laravel7
   
   echo [STEP] Initialize git
   cd /d c:\incoming\php-laravel7\src
   git init
   git add .env.example
   git commit -m "a commit"
   
   git config user.name "Mirco Babin"
   git config user.email "mirco@..."
   
   echo [STEP] Start GitNoob
   start "" "%~dp0\..\..\bin\release\gitnoob.exe" "c:\incoming\php-laravel7\GitNoob\GitNoob.ini"
   
   pause