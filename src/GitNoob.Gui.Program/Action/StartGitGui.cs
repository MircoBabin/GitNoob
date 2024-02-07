using GitNoob.Utils;
using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public class StartGitGui : Action
    {
        public StartGitGui(ProgramWorkingDirectory Config) : base(Config) { }

        private static string _cacheExecutable = null;
        public static string GetExecutable()
        {
            if (_cacheExecutable == null)
            {
                try
                {
                    _cacheExecutable = FileUtils.FindExePath("git-gui.exe");
                }
                catch
                {
                    _cacheExecutable = string.Empty;
                }
            }

            return _cacheExecutable;
        }

        public override bool isStartable()
        {
            return true;
        }

        public override Icon icon()
        {
            return Resources.getIcon("git gui");
        }

        public override void execute()
        {
            //direct execution, because also started as clickable link or button inside a Remedy
            //don't use StepsExecutor

            var executable = GetExecutable();
            if (string.IsNullOrEmpty(executable)) return;

            var batFile = new BatFile(config.visualizerShowException, "run-gitgui", BatFile.RunAsType.runAsInvoker, BatFile.WindowType.hideWindow, "GitNoob - Git Gui",
                config.Project, config.ProjectWorkingDirectory,
                config.PhpIni);
            batFile.AppendLine("start \"Git-Gui\" \"" + executable + "\"");
            batFile.AppendLine("exit /b 0");

            batFile.Start();
        }
    }
}

