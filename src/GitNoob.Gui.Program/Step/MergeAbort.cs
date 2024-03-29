﻿using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class MergeAbort : Step
    {
        private bool _presentRemedy;
        public MergeAbort(bool PresentRemedy) : base()
        {
            _presentRemedy = PresentRemedy;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - aborting merge";

            var result = StepsExecutor.Config.Git.MergeAbort(null);

            var message = new VisualizerMessageWithLinks("Aborting merge failed.");

            if (!_presentRemedy)
            {
                return true;
            }

            if (result.IsGitDisasterHappening != false)
            {
                FailureRemedy = new Remedy.MessageGitDisaster(this, message, result);
                return false;
            }

            if (result.ErrorNotMerging)
            {
                return true;
            }

            if (!result.Aborted)
            {
                FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                return false;
            }

            return true;
        }
    }
}

