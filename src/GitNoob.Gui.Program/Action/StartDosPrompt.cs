using GitNoob.Utils;
using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public abstract class StartDosPrompt : Action
    {
        public StartDosPrompt(ProgramWorkingDirectory Config) : base(Config) { }

        private static string _cacheExectable = null;
        private static string GetExecutable()
        {
            if (_cacheExectable == null)
            {
                try
                {
                    _cacheExectable = FileUtils.FindExePath("%ComSpec%");
                }
                catch
                {
                    _cacheExectable = string.Empty;
                }
            }

            return _cacheExectable;
        }

        public override bool isStartable()
        {
            return true;
        }

        public override Icon icon()
        {
            var cmd = GetExecutable();
            return ImageUtils.LoadIconForFile(cmd);
        }

        protected void executeDosPrompt(bool asAdministrator)
        {
            var executable = GetExecutable();
            if (string.IsNullOrEmpty(executable)) return;

            BatFile.StartDosPrompt(config.visualizerShowException, asAdministrator, config.Project, config.ProjectWorkingDirectory, config.PhpIni);
        }
    }
}
