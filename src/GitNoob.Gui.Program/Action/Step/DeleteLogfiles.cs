using System;

namespace GitNoob.Gui.Program.Action.Step
{
    public class DeleteLogfiles : Step
    {
        private string _path;

        public DeleteLogfiles(string Path) : base()
        {
            _path = Path;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - deleting logfiles from " + _path;

            try
            {
                Utils.FileUtils.DirectoryDeleteFilesExcludingDotGitFiles(_path);
            }
            catch (Exception ex)
            {
                FailureRemedy = new Remedy.MessageException(this, new VisualizerMessageWithLinks(), ex);
                return false;
            }

            return true;
        }
    }
}
