using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace GitNoob.Gui.Program.Action.Step
{
    public class StartApache : Step
    {
        public StartApache() : base() { }

        protected override bool run()
        {
            if (StepsExecutor.Config.ProjectWorkingDirectory.Apache.Port == 0) return true;

            if (string.IsNullOrEmpty(StepsExecutor.Config.ProjectWorkingDirectory.Apache.ApachePath)) return true;
            var apacheBinPath = Path.Combine(StepsExecutor.Config.ProjectWorkingDirectory.Apache.ApachePath, "bin");
            if (!File.Exists(Path.Combine(apacheBinPath, "httpd.exe"))) return true;

            BusyMessage = "Busy - starting Apache";

            var running = false;
            try
            {
                if (File.Exists(StepsExecutor.Config.ApacheConf.PidFullFilename))
                {
                    var fi = new FileInfo(StepsExecutor.Config.ApacheConf.PidFullFilename);
                    if (fi.Length < 256)
                    {
                        var processno = Convert.ToInt32(File.ReadAllText(StepsExecutor.Config.ApacheConf.PidFullFilename, Encoding.ASCII).Trim());

                        try
                        {
                            var process = Process.GetProcessById(processno);
                            var processFilename = process.MainModule.FileName.ToString();
                            if (processFilename.ToLowerInvariant().EndsWith("\\httpd.exe")) running = true;
                        }
                        catch (ArgumentException) { }
                    }
                }
            }
            catch { }

            if (!running)
            {
                StringBuilder batContents = new StringBuilder();
                batContents.AppendLine("@echo off");
                batContents.AppendLine("title Apache for " + StepsExecutor.Config.ApacheConf.ProjectnameASCII);
                batContents.AppendLine("echo This dosprompt is running Apache webserver on port " + StepsExecutor.Config.ProjectWorkingDirectory.Apache.Port + ".");
                batContents.AppendLine("echo.");
                batContents.AppendLine("echo Browse to " + StepsExecutor.Config.ProjectWorkingDirectory.Webpage.GetHomepageUrl(StepsExecutor.Config.ProjectWorkingDirectory.Apache.Port));
                batContents.AppendLine("echo.");
                batContents.AppendLine("echo Do not close this dosprompt.");
                batContents.AppendLine("cd /D \"" + apacheBinPath + "\"");
                batContents.AppendLine("httpd.exe -f \"" + StepsExecutor.Config.ApacheConf.ConfFullFilename + "\"");
                batContents.AppendLine("if errorlevel 1 pause");
                batContents.AppendLine("exit /b 0");

                var path = Utils.FileUtils.TempDirectoryForProject(StepsExecutor.Config.Project, StepsExecutor.Config.ProjectWorkingDirectory);
                var batFile = Path.Combine(path, "run-apache.bat");
                File.WriteAllText(batFile, batContents.ToString(), Encoding.ASCII);

                var info = new ProcessStartInfo
                {
                    WorkingDirectory = Path.Combine(StepsExecutor.Config.ProjectWorkingDirectory.Apache.ApachePath, "bin"),
                    FileName = batFile,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Minimized,
                };

                if (StepsExecutor.Config.ProjectWorkingDirectory.ProjectType != null &&
                    StepsExecutor.Config.ProjectWorkingDirectory.ProjectType.Capabilities.NeedsPhp)
                {
                    info.EnvironmentVariables["PHPRC"] = StepsExecutor.Config.PhpIni.IniPath; /* Directory containing php.ini */
                    info.EnvironmentVariables["Path"] = info.EnvironmentVariables["Path"] + ";" + StepsExecutor.Config.ProjectWorkingDirectory.Php.Path;
                }

                Process.Start(info);
            }

            return true;
        }
    }
}
