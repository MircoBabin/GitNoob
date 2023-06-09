using GitNoob.Utils;
using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public class StartExplorer : Action
    {
        public StartExplorer(ProgramWorkingDirectory Config) : base(Config) { }

        private static string _cacheExecutable = null;
        private static string GetExecutable()
        {
            if (_cacheExecutable == null)
            {
                try
                {
                    _cacheExecutable = FileUtils.FindExePath("%windir%\\explorer.exe");
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
            var explorer = GetExecutable();
            return Utils.ImageUtils.LoadIconForFile(explorer);
        }

        public override void execute()
        {
            var path = config.ProjectWorkingDirectory.Path.ToString();

            Utils.BatFile.StartWindowsExplorer(path, config.Project, config.ProjectWorkingDirectory, config.PhpIni);
        }
    }
}
