using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public class StartExplorer : IAction
    {
        private static string _cacheExecutable = null;
        private static string GetExecutable()
        {
            if (_cacheExecutable == null)
            {
                _cacheExecutable = Utils.FileUtils.FindExePath("%windir%\\explorer.exe");
            }

            return _cacheExecutable;
        }

        private ProgramWorkingDirectory Config;
        public StartExplorer(ProgramWorkingDirectory Config)
        {
            this.Config = Config;
        }

        public Icon icon()
        {
            var explorer = GetExecutable();
            return Utils.ImageUtils.LoadIconForFile(explorer);
        }

        public void execute()
        {
            var path = Config.ProjectWorkingDirectory.Path;
            if (!path.EndsWith("\\")) path = path + "\\";

            System.Diagnostics.Process.Start(path);
        }
    }
}
