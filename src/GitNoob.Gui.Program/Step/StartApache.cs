using GitNoob.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace GitNoob.Gui.Program.Step
{
    public class StartApache : Step
    {
        public StartApache() : base() { }

        protected override bool run()
        {
            if (StepsExecutor.Config.ProjectWorkingDirectory.Apache.Port == 0) return true;

            if (StepsExecutor.Config.ProjectWorkingDirectory.Apache.ApachePath.isEmpty()) return true;
            var apacheBinPath = Path.Combine(StepsExecutor.Config.ProjectWorkingDirectory.Apache.ApachePath.ToString(), "bin");
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
                var confFilename = StepsExecutor.Config.ApacheConf.ConfFullFilename;
                var batFile = new BatFile(StepsExecutor.Config.visualizerShowException, "run-apache", BatFile.RunAsType.runAsInvoker, BatFile.WindowType.showWindow, "Apache for " + StepsExecutor.Config.ApacheConf.ProjectnameASCII,
                    StepsExecutor.Config.Project, StepsExecutor.Config.ProjectWorkingDirectory,
                    StepsExecutor.Config.PhpIni,
                    apacheBinPath);
                batFile.AppendLine("echo This dosprompt is running Apache webserver on port " + StepsExecutor.Config.ProjectWorkingDirectory.Apache.Port + ".");
                batFile.AppendLine("echo.");
                batFile.AppendLine("echo Browse to " + StepsExecutor.Config.ProjectWorkingDirectory.Webpage.GetHomepageUrl(StepsExecutor.Config.ProjectWorkingDirectory.Apache.Port));
                batFile.AppendLine("echo.");
                batFile.AppendLine("echo Apache configfile: " + confFilename);
                batFile.AppendLine("echo Apache path: " + apacheBinPath);
                batFile.AppendLine("httpd.exe -v");
                batFile.AppendLine("echo.");
                batFile.AppendLine("echo Do not close this dosprompt.");
                batFile.AppendLine("httpd.exe -f \"" + confFilename + "\"");
                batFile.AppendLine("if errorlevel 1 pause");
                batFile.AppendLine("exit /b 0");

                batFile.Start();
            }

            return true;
        }
    }
}
