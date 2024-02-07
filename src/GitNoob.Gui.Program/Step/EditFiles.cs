using GitNoob.Gui.Visualizer;
using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Step
{
    public class EditFiles : Step
    {
        private IEnumerable<string> _files;
        public EditFiles(IEnumerable<string> files) : base()
        {
            _files = files;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - opening files";

            try
            {
                Utils.BatFile.StartEditor(null, _files,
                    StepsExecutor.Config.Project, StepsExecutor.Config.ProjectWorkingDirectory, StepsExecutor.Config.PhpIni);
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
