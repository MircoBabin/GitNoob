using GitNoob.Gui.Visualizer;
using System;

namespace GitNoob.Gui.Program.Step
{
    public class TouchTimestampsOnCurrentBranch : Step
    {
        private DateTime _toTime;
        public TouchTimestampsOnCurrentBranch(DateTime toTime) : base()
        {
            _toTime = toTime;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - setting timestamps";

            var result = StepsExecutor.Config.Git.TouchCommitAndAuthorTimestampsOfCurrentBranch(_toTime);

            var message = new VisualizerMessageWithLinks("Setting timestamps failed.");

            if (result.IsGitDisasterHappening != false)
            {
                FailureRemedy = new Remedy.MessageGitDisaster(this, message, result);
                return false;
            }

            if (!result.NoCommitsToTouch && !result.Touched)
            {
                FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                return false;
            }

            return true;
        }
    }
}

