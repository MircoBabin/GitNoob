using GitNoob.Gui.Program.Utils;
using System.Collections.Generic;
using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public class StartGitk : Action, IAction
    {
        private List<string> branches;
        private string focusOnCommitId;
        private List<string> filenames;

        public StartGitk(StepsExecutor.StepConfig Config, List<string> branches, string focusOnCommitId, List<string> filenames = null) : base(Config) 
        {
            this.branches = branches;
            this.focusOnCommitId = focusOnCommitId;
            this.filenames = filenames;
        }

        private static string _cacheExecutable = null;
        public static string GetExecutable()
        {
            if (_cacheExecutable == null)
            {
                try
                {
                    _cacheExecutable = Utils.FileUtils.FindExePath("gitk.exe");
                }
                catch
                {
                    _cacheExecutable = string.Empty;
                }
            }

            return _cacheExecutable;
        }

        public Icon icon()
        {
            return Utils.Resources.getIcon("gitk");
        }

        public void execute()
        {
            //direct execution, because also started as clickable link or button inside a Remedy
            //don't use StepsExecutor

            var executable = GetExecutable();
            if (string.IsNullOrEmpty(executable)) return;

            var batFile = new BatFile("run-gitkall", BatFile.RunAsType.runAsInvoker, BatFile.WindowType.hideWindow, "GitNoob - Git History",
                stepConfig.Config.Project, stepConfig.Config.ProjectWorkingDirectory,
                stepConfig.Config.PhpIni);
            batFile.Append("start \"Git-Gitk-Branches\" \"" + executable + "\"");
            if (!string.IsNullOrWhiteSpace(focusOnCommitId))
            {
                batFile.Append(" \"--select-commit=" + focusOnCommitId + "\"");
            }
            if (branches != null)
            {
                foreach(var branch in branches)
                {
                    batFile.Append(" \"" + branch + "\"");
                }
            }
            batFile.Append(" -- ");
            if (filenames != null)
            {
                foreach(var filename in filenames)
                {
                    batFile.Append(" \"" + filename + "\"");
                }
            }
            batFile.AppendLine(string.Empty);

            batFile.AppendLine("exit /b 0");

            batFile.Start();
        }
    }
}

