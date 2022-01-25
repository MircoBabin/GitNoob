using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public class ExecuteStartBrowser : Action, IAction
    {
        public ExecuteStartBrowser(StepsExecutor.StepConfig Config) : base(Config) { }

        private static string _cacheHttpHandler = null;
        public static string GetHttpHandler()
        {
            if (_cacheHttpHandler == null)
            {
                //Query from HKEY_CLASSES_ROOT\http\shell\open\command
                string openCommand = String.Empty;
                using (var key = Registry.ClassesRoot.OpenSubKey("http\\shell\\open\\command", false))
                {
                    if (key != null) openCommand = ((string)key.GetValue("")).Trim();
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

                _cacheHttpHandler = Utils.FileUtils.FindExePath(executable);
            }

            return _cacheHttpHandler;
        }

        private string GetUrl()
        {
            return stepConfig.Config.ProjectWorkingDirectory.Webpage.GetHomepageUrl(stepConfig.Config.ProjectWorkingDirectory.Apache.Port);
        }

        public bool isStartable()
        {
            return (GetUrl() != null);
        }

        public Icon icon()
        {
            var http = GetHttpHandler();
            return Utils.ImageUtils.LoadIconForFile(http);
        }

        public void execute()
        {
            if (stepConfig.Visualizer.isFrontendLocked()) return;

            var executor = new StepsExecutor.StepsExecutor(stepConfig, new List<StepsExecutor.IExecutableByStepsExecutor>()
            {
                new Step.StartApache(),
                new Step.StartBrowser(GetUrl()),
            });
            executor.execute();
        }
    }
}
