using System.Drawing;
using System.IO;

namespace GitNoob.Gui.Program.Action
{
    public class StartWorkspace : IAction
    {
        private ProgramWorkingDirectory Config;
        public StartWorkspace(ProgramWorkingDirectory Config)
        {
            this.Config = Config;
        }

        private string GetFile()
        {
            if (Config.ProjectWorkingDirectory.Editor.WorkspaceFilename.isEmpty()) return null;

            return Config.ProjectWorkingDirectory.Editor.WorkspaceFilename.ToString();
        }

        public bool isStartable()
        {
            var file = GetFile();
            if (file == null) return false;
            if (!File.Exists(file)) return false;

            return true;
        }

        public Icon icon()
        {
            return Utils.ImageUtils.LoadIconForFile(Config.ProjectWorkingDirectory.Editor.WorkspaceFilename.ToString());
        }

        public void execute()
        {
            System.Diagnostics.Process.Start(GetFile());
        }
    }
}
