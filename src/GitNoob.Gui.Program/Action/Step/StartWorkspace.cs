using System.Diagnostics;
using System.IO;
using System.Text;

namespace GitNoob.Gui.Program.Action.Step
{
    public class StartWorkspace : Step
    {
        private string _filename;
        private bool _asAdministrator;
        public StartWorkspace(string filename, bool asAdministrator) : base()
        {
            _filename = filename;
            _asAdministrator = asAdministrator;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - starting workspace " + _filename;

            if (_asAdministrator)
            {
                StringBuilder batContents = new StringBuilder();
                batContents.AppendLine("@echo off");
                batContents.AppendLine("start \"Workspace\" \"" + _filename + "\"");

                var path = Utils.FileUtils.TempDirectoryForProject(StepsExecutor.Config.Project, StepsExecutor.Config.ProjectWorkingDirectory);
                var batFile = Path.Combine(path, "run-workspace.bat");
                File.WriteAllText(batFile, batContents.ToString(), Encoding.ASCII);

                var info = new ProcessStartInfo
                {
                    FileName = batFile,
                    UseShellExecute = true,
                    Verb = "runas",
                    WindowStyle = ProcessWindowStyle.Hidden,
                };

                Process.Start(info);
            }
            else
            {
                Process.Start(_filename);
            }

            return true;
        }
    }
}
