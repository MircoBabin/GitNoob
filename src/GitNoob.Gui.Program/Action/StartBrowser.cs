using GitNoob.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public class StartBrowser : Action
    {
        public StartBrowser(ProgramWorkingDirectory Config) : base(Config) { }

        private static string _cacheHttpHandler = null;
        public static string GetHttpHandler()
        {
            if (_cacheHttpHandler == null)
            {
                //Query progid from HKEY_CURRENT_USER
                string progid = String.Empty;
                using (var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\Shell\\Associations\\UrlAssociations\\http\\UserChoice", false))
                {
                    if (key != null) progid = ((string)key.GetValue("Progid")).Trim();
                }

                //Progid to executable
                string openCommand = String.Empty;
                if (!String.IsNullOrWhiteSpace(progid))
                {
                    using (var key = Registry.ClassesRoot.OpenSubKey(progid + "\\shell\\open\\command"))
                    {
                        if (key != null) openCommand = ((string)key.GetValue(null)).Trim();
                    }
                }

                //Split {executable} {commandline}
                string executable = String.Empty;
                if (openCommand.StartsWith("\""))
                {
                    executable = openCommand.Substring(1, openCommand.IndexOf('"', 1) - 1);
                }
                else
                {
                    executable = openCommand.Substring(0, openCommand.IndexOf(' '));
                }

                try
                {
                    _cacheHttpHandler = Utils.FileUtils.FindExePath(executable);
                }
                catch
                {
                    _cacheHttpHandler = string.Empty;
                }
            }

            return _cacheHttpHandler;
        }

        private string GetUrl()
        {
            return config.ProjectWorkingDirectory.Webpage.GetHomepageUrl(config.ProjectWorkingDirectory.Apache.Port);
        }

        public override bool isStartable()
        {
            return (GetUrl() != null);
        }

        public override Icon icon()
        {
            var http = GetHttpHandler();
            return ImageUtils.LoadIconForFile(http);
        }

        public override void execute()
        {
            if (config.Visualizer.isFrontendLocked()) return;

            var executor = new StepsExecutor.StepsExecutor(config, new List<StepsExecutor.IExecutableByStepsExecutor>()
            {
                new Step.StartApache(),
                new Step.StartBrowser(GetUrl()),
            });
            executor.execute();
        }
    }
}
