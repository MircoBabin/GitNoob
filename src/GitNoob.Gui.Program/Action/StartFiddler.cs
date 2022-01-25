using System;
using System.Drawing;
using System.IO;

namespace GitNoob.Gui.Program.Action
{
    public class StartFiddler : IAction
    {
        private static string _cacheExecutable = null;
        private static string GetExecutable()
        {
            if (_cacheExecutable == null)
            {
                _cacheExecutable = Utils.FileUtils.FindExePath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs\\Fiddler\\Fiddler.exe"));
            }

            return _cacheExecutable;
        }

        public bool isStartable()
        {
            var fiddler = GetExecutable();
            if (fiddler == null) return false;

            if (!File.Exists(fiddler)) return false;

            return true;
        }

        public Icon icon()
        {
            var fiddler = GetExecutable();
            return Utils.ImageUtils.LoadIconForFile(fiddler);
        }

        public void execute()
        {
            System.Diagnostics.Process.Start(GetExecutable());
        }
    }
}
