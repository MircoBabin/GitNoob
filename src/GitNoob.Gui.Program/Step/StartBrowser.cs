namespace GitNoob.Gui.Program.Step
{
    public class StartBrowser : Step
    {
        private string _url;
        public StartBrowser(string url) : base()
        {
            BusyMessage = "Busy - starting browser to " + _url;

            _url = url;
        }

        protected override bool run()
        {
            Utils.BatFile.StartWebBrowser(_url, StepsExecutor.Config.Project, StepsExecutor.Config.ProjectWorkingDirectory, StepsExecutor.Config.PhpIni);

            return true;
        }
    }
}
