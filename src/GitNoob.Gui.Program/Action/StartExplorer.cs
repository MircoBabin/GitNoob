using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public class StartExplorer : Action, IAction
    {
        public StartExplorer(StepsExecutor.StepConfig Config) : base(Config) { }

        private static string _cacheExecutable = null;
        private static string GetExecutable()
        {
            if (_cacheExecutable == null)
            {
                try
                {
                    _cacheExecutable = Utils.FileUtils.FindExePath("%windir%\\explorer.exe");
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
            var explorer = GetExecutable();
            return Utils.ImageUtils.LoadIconForFile(explorer);
        }

        public void execute()
        {
            var path = stepConfig.Config.ProjectWorkingDirectory.Path.ToString();

            Utils.BatFile.StartWindowsExplorer(path, stepConfig.Config.Project, stepConfig.Config.ProjectWorkingDirectory, stepConfig.Config.PhpIni);
        }
    }
}
