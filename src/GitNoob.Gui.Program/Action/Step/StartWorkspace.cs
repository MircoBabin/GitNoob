using GitNoob.Gui.Program.Utils;

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

            var batFile = new BatFile("run-workspace", _asAdministrator,
                StepsExecutor.Config.Project, StepsExecutor.Config.ProjectWorkingDirectory,
                StepsExecutor.Config.PhpIni);
            batFile.AppendLine("start \"Workspace\" \"" + _filename + "\"");
            batFile.AppendLine("exit /b 0");

            batFile.Execute();

            return true;
        }
    }
}
