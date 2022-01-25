namespace GitNoob.Gui.Program.Action.Step
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
            System.Diagnostics.Process.Start(_url);

            return true;
        }
    }
}
