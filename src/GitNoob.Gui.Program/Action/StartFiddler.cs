using System;
using System.Drawing;
using System.IO;

namespace GitNoob.Gui.Program.Action
{
    public class StartFiddler : Action
    {
        public StartFiddler(ProgramWorkingDirectory Config) : base(Config) { }

        private static string _cacheExecutable = null;
        private static string GetExecutable()
        {
            if (_cacheExecutable == null)
            {
                try
                {
                    _cacheExecutable = Utils.FileUtils.FindExePath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs\\Fiddler\\Fiddler.exe"));
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
            var fiddler = GetExecutable();
            if (string.IsNullOrEmpty(fiddler)) return false;

            if (!File.Exists(fiddler)) return false;

            return true;
        }

        public override Icon icon()
        {
            var fiddler = GetExecutable();
            return Utils.ImageUtils.LoadIconForFile(fiddler);
        }

        public override void execute()
        {
            var executable = GetExecutable();
            if (string.IsNullOrEmpty(executable)) return;

            Utils.BatFile.StartExecutable(executable, null, 
                config.Project, config.ProjectWorkingDirectory, config.PhpIni);
        }
    }
}
