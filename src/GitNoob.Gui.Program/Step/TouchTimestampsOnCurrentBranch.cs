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

            if (result.ErrorRebaseInProgress || result.ErrorMergeInProgress)
            {
                FailureRemedy = new Remedy.MessageRebasingOrMerging(this, message, result.ErrorRebaseInProgress, result.ErrorMergeInProgress);
                return false;
            }

            if (result.ErrorDetachedHead)
            {
                FailureRemedy = new Remedy.MessageDetachedHead(this, message);
                return false;
            }

            if (result.ErrorWorkingTreeChanges || result.ErrorStagedUncommittedFiles)
            {
                FailureRemedy = new Remedy.MessageChanges(this, message, result.ErrorWorkingTreeChanges, result.ErrorStagedUncommittedFiles);
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

