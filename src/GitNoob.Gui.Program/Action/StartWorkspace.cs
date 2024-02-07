using GitNoob.Utils;
using System.Drawing;
using System.IO;

namespace GitNoob.Gui.Program.Action
{
    public class StartWorkspace : Action
    {
        public StartWorkspace(ProgramWorkingDirectory Config) : base(Config) { }

        private string GetFile()
        {
            if (config.ProjectWorkingDirectory.Editor.WorkspaceFilename.isEmpty()) return null;

            return config.ProjectWorkingDirectory.Editor.WorkspaceFilename.ToString();
        }

        public override bool isStartable()
        {
            var file = GetFile();
            if (file == null) return false;
            if (!File.Exists(file)) return false;

            return true;
        }

        public override Icon icon()
        {
            return Utils.ImageUtils.LoadIconForFile(config.ProjectWorkingDirectory.Editor.WorkspaceFilename.ToString());
        }

        public override void execute()
        {
            //direct execution, because also started as clickable link or button inside a Remedy
            //don't use StepsExecutor

            if (!isStartable()) return;

            string filename = GetFile();
            bool asAdministrator = config.ProjectWorkingDirectory.Editor.WorkspaceRunAsAdministrator.Value;

            var batFile = new BatFile(config.visualizerShowException, "run-workspace", (asAdministrator ? BatFile.RunAsType.runAsAdministrator : BatFile.RunAsType.runAsInvoker), BatFile.WindowType.hideWindow, "GitNoob - Workspace",
                config.Project, config.ProjectWorkingDirectory,
                config.PhpIni);
            batFile.AppendLine("start \"Workspace\" \"" + filename + "\"");
            batFile.AppendLine("exit /b 0");

            batFile.Start();
        }
    }
}
