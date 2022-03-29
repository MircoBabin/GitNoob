using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public class StartGitGui : IAction
    {
        private static string _cacheExecutable = null;
        public static string GetExecutable()
        {
            if (_cacheExecutable == null)
            {
                _cacheExecutable = Utils.FileUtils.FindExePath("git-gui.exe");
            }

            return _cacheExecutable;
        }

        private ProgramWorkingDirectory Config;
        public StartGitGui(ProgramWorkingDirectory Config)
        {
            this.Config = Config;
        }

        public Icon icon()
        {
            return Utils.Resources.getIcon("git gui");
        }

        public void execute()
        {
            var info = new System.Diagnostics.ProcessStartInfo
            {
                WorkingDirectory = Config.ProjectWorkingDirectory.Path.ToString(),
                FileName = GetExecutable(),
                UseShellExecute = false,
            };

            System.Diagnostics.Process.Start(info);
        }
    }
}
