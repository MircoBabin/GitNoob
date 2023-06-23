using GitNoob.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public class StartGitk : Action
    {
        public StartGitk(ProgramWorkingDirectory Config) : base(Config) { }

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

        public override bool isStartable()
        {
            return true;
        }

        public override Icon icon()
        {
            return Utils.Resources.getIcon("gitk");
        }

        public override void execute()
        {
            throw new Exception("StartGitk.execute() is not implemented. Use executeGitk().");
        }

        public void executeGitk(List<string> branches, string focusOnCommitId = null, List<string> filenames = null)
        {
            //direct execution, because also started as clickable link or button inside a Remedy
            //don't use StepsExecutor

            var executable = GetExecutable();
            if (string.IsNullOrEmpty(executable)) return;

            var batFile = new BatFile("run-gitkall", BatFile.RunAsType.runAsInvoker, BatFile.WindowType.hideWindow, "GitNoob - Git History",
                config.Project, config.ProjectWorkingDirectory,
                config.PhpIni);

            {
                batFile.Append("start \"Git-Gitk-Branches\" \"" + executable + "\"");

                if (!string.IsNullOrWhiteSpace(focusOnCommitId))
                {
                    batFile.Append(" \"--select-commit=" + focusOnCommitId + "\"");
                }

                if (branches != null)
                {
                    foreach (var branch in branches)
                    {
                        if (!string.IsNullOrWhiteSpace(branch))
                            batFile.Append(" \"" + branch + "\"");
                    }
                }
                else
                {
                    batFile.Append(" --all");
                }

                batFile.Append(" -- ");

                if (filenames != null)
                {
                    foreach (var filename in filenames)
                    {
                        batFile.Append(" \"" + filename + "\"");
                    }
                }

                batFile.AppendLine(string.Empty);
            }

            batFile.AppendLine("exit /b 0");

            batFile.Start();
        }
    }
}

