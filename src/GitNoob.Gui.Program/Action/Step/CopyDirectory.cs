using System;
using System.IO;

namespace GitNoob.Gui.Program.Action.Step
{
    public class CopyDirectory : Step
    {
        private string _copyTo;

        public CopyDirectory(string CopyTo) : base()
        {
            _copyTo = CopyTo;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - copying directory \"" + StepsExecutor.Config.ProjectWorkingDirectory.Path + "\" to \"" + Path.GetFileName(_copyTo) + "\"";

            try
            {
              Utils.FileUtils.DirectoryCopy(StepsExecutor.Config.ProjectWorkingDirectory.Path, _copyTo, true);
            }
            catch (Exception ex)
            {
                FailureRemedy = new Remedy.MessageException(this, new MessageWithLinks(), ex);
                return false;
            }

            return true;
        }
    }
}
