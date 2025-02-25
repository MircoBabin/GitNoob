﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GitNoob.ProjectTypes
{
    public class PhpLaravel
    {
        private static Config.IProjectType_Capabilities _capabilities = new Config.IProjectType_Capabilities()
        {
            NeedsPhp = true,
            CapableOfClearAndBuildCache = true,
        };
        public Config.IProjectType_Capabilities Capabilities { get { return _capabilities; } }

        public string OverridePhpTempPath(string TempPath, string GIT_ROOT_DIR)
        {
            string storageTmpPhp_tempdir = Path.Combine(GIT_ROOT_DIR, "storage\\tmp\\php-tempdir");
            if (!Directory.Exists(storageTmpPhp_tempdir))
            {
                try
                {
                    Directory.CreateDirectory(storageTmpPhp_tempdir);
                }
                catch
                {
                    return TempPath;
                }
            }

            return storageTmpPhp_tempdir;
        }

        public string OverridePhpLogPath(string LogPath, string GIT_ROOT_DIR)
        {
            return Path.Combine(GIT_ROOT_DIR, "storage\\logs");
        }

        public IEnumerable<string> GetConfigurationFiles()
        {
            //Relative to the git root directory.
            return new List<string>()
            {
                ".env",
            };
        }

        public IEnumerable<string> GetLogfilesPaths()
        {
            //Relative to the git root directory.
            return new List<string>()
            {
                "storage\\logs",
            };
        }

        private Config.IProjectType_ActionResult laravelCache(Config.IExecutor executor, bool buildCache)
        {
            //Current directory is the git root directory.
            string php_exe = "\"" + executor.GetPhpExe() + "\"";

            string bat =
                "@echo off" + Environment.NewLine +
                "    rem echo." + Environment.NewLine +
                "    rem echo [Php.exe information]" + Environment.NewLine +
                "    rem " + php_exe + " -i" + Environment.NewLine +
                Environment.NewLine +
                "    if exist composer.phar goto composer_installed" + Environment.NewLine +
                ":composer_install" + Environment.NewLine +
                "    echo." + Environment.NewLine +
                "    echo [Composer download without SHA384 check]" + Environment.NewLine +
                "    rem https://getcomposer.org/download/ without SHA384 check" + Environment.NewLine +
                "    " + php_exe + " -r \"copy('https://getcomposer.org/installer', 'composer-setup.php'); \"" + Environment.NewLine +
                "    " + php_exe + " composer-setup.php" + Environment.NewLine +
                "    " + php_exe + " -r \"unlink('composer-setup.php'); \"" + Environment.NewLine +
                Environment.NewLine +
                "    if exist composer.phar goto composer_installed" + Environment.NewLine +
                "    echo." + Environment.NewLine +
                "    composer.phar not found" + Environment.NewLine +
                "    exit /b 100" + Environment.NewLine +
                Environment.NewLine +
                ":composer_installed" + Environment.NewLine +
                "    echo." + Environment.NewLine +
                "    echo [Delete .tmp files from the cache: bootstrap\\cache\\*.tmp]" + Environment.NewLine +
                "    attrib -r bootstrap\\cache\\*.tmp >nul 2>&1" + Environment.NewLine +
                "    del /q bootstrap\\cache\\*.tmp >nul 2>&1" + Environment.NewLine +
                "    if not exist bootstrap\\cache\\*.tmp goto clear_packages" + Environment.NewLine +
                Environment.NewLine +
                "    echo Deleting bootstrap\\cache\\*.tmp failed." + Environment.NewLine +
                "    exit /b 106" + Environment.NewLine +
                Environment.NewLine +
                ":clear_packages" + Environment.NewLine +
                "    echo." + Environment.NewLine +
                "    echo [Delete packages cache: bootstrap\\cache\\packages.php]" + Environment.NewLine +
                "    attrib -r bootstrap\\cache\\packages.php >nul 2>&1" + Environment.NewLine +
                "    del /q bootstrap\\cache\\packages.php >nul 2>&1" + Environment.NewLine +
                "    if not exist bootstrap\\cache\\packages.php goto dump_autoload" + Environment.NewLine +
                Environment.NewLine +
                "    echo Deleting bootstrap\\cache\\packages.php failed." + Environment.NewLine +
                "    exit /b 107" + Environment.NewLine +
                Environment.NewLine +
                ":dump_autoload" + Environment.NewLine +
                "    echo." + Environment.NewLine +
                "    echo [Composer dump-autoload]" + Environment.NewLine +
                "    " + php_exe + " composer.phar dump-autoload --no-dev --no-ansi --no-interaction" + Environment.NewLine +
                "    echo [errorlevel=%errorlevel%]" + Environment.NewLine +
                "    if errorlevel 1 exit /b 101" + Environment.NewLine +
                "    if exist bootstrap\\cache\\packages.php goto clear_cache" + Environment.NewLine +
                Environment.NewLine +
                "    echo Creating packages cache in bootstrap\\cache\\packages.php failed." + Environment.NewLine +
                "    exit /b 108" + Environment.NewLine +
                Environment.NewLine +
                ":clear_cache" + Environment.NewLine +
                "    echo." + Environment.NewLine +
                "    echo [Artisan cache:clear]" + Environment.NewLine +
                "    " + php_exe + " artisan cache:clear" + Environment.NewLine +
                "    echo [errorlevel=%errorlevel%]" + Environment.NewLine +
                "    if errorlevel 1 exit /b 102" + Environment.NewLine +
                Environment.NewLine +
                "    echo." + Environment.NewLine +
                "    echo [Artisan config:clear]" + Environment.NewLine +
                "    " + php_exe + " artisan config:clear" + Environment.NewLine +
                "    echo [errorlevel=%errorlevel%]" + Environment.NewLine +
                "    if errorlevel 1 exit /b 103" + Environment.NewLine +
                Environment.NewLine +
                "    echo." + Environment.NewLine +
                "    echo [Artisan route:clear]" + Environment.NewLine +
                "    " + php_exe + " artisan route:clear" + Environment.NewLine +
                "    echo [errorlevel=%errorlevel%]" + Environment.NewLine +
                "    if errorlevel 1 exit /b 104" + Environment.NewLine +
                Environment.NewLine +
                "    echo." + Environment.NewLine +
                "    echo [Artisan view:clear]" + Environment.NewLine +
                "    " + php_exe + " artisan view:clear" + Environment.NewLine +
                "    echo [errorlevel=%errorlevel%]" + Environment.NewLine +
                "    if errorlevel 1 exit /b 105" + Environment.NewLine +
                Environment.NewLine;

            if (buildCache)
            {
                bat +=
                "    echo." + Environment.NewLine +
                "    echo [Artisan route:cache]" + Environment.NewLine +
                "    " + php_exe + " artisan route:cache" + Environment.NewLine +
                "    echo [errorlevel=%errorlevel%]" + Environment.NewLine +
                "    if errorlevel 1 exit /b 200" + Environment.NewLine +
                Environment.NewLine;
            }

            bat +=
                ":end" + Environment.NewLine +
                "    echo." + Environment.NewLine +
                "    echo [GIT status]" + Environment.NewLine +
                "    git status --porcelain" + Environment.NewLine +
                "    exit /b 0" + Environment.NewLine;

            var executorResult = executor.ExecuteBatFile(bat);

            StringBuilder message = new StringBuilder();
            bool success = true;
            foreach (var line in executorResult.StandardError.Split('\n'))
            {
                if (line.ToLowerInvariant().Contains("does not comply with psr-4"))
                {
                    success = false;
                    message.AppendLine(line.Trim());
                }
            }
            switch(executorResult.ExitCode)
            {
                case 0:
                    break;

                case 100:
                    message.AppendLine("Error downloading composer.phar.");
                    break;

                case 101:
                    message.AppendLine("Error during composer dump-autoload.");
                    break;

                case 102:
                    message.AppendLine("Error during artisan cache:clear.");
                    break;

                case 103:
                    message.AppendLine("Error during artisan config:clear.");
                    break;

                case 104:
                    message.AppendLine("Error during artisan route:clear.");
                    break;

                case 105:
                    message.AppendLine("Error during artisan view:clear.");
                    break;

                case 106:
                    message.AppendLine("Error during remove .tmp files from cache.");
                    break;

                case 107:
                    message.AppendLine("Error during remove packages cache.");
                    break;

                case 108:
                    message.AppendLine("Error during create packages cache.");
                    break;

                case 200:
                    message.AppendLine("Error during artisan route:cache. There must be at least one route, and each route must point to a class and function name. Anonymous functions must not be used.");
                    break;

                default:
                    message.AppendLine("Unknown exitcode " + executorResult.ExitCode);
                    break;
            }

            return new Config.IProjectType_ActionResult()
            {
                Result = (success && executorResult.ExitCode == 0),
                Message = message.ToString(),
                StandardOutput = executorResult.StandardOutput,
                StandardError = executorResult.StandardError,
            };
        }

        public Config.IProjectType_ActionResult ClearCache(Config.IExecutor executor)
        {
            return laravelCache(executor, false);
        }

        public Config.IProjectType_ActionResult BuildCache(Config.IExecutor executor)
        {
            return laravelCache(executor, true);
        }
    }
}
