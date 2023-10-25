using GitNoob.Gui.Visualizer;
using System;

namespace GitNoob.Gui.Program.Step
{
    public class CherryPick : Step
    {
        private string _commitId;

        public CherryPick(string commitId) : base()
        {
            _commitId = commitId;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - cherry picking one commit";

            var result = StepsExecutor.Config.Git.CherryPick(_commitId);

            var message = new VisualizerMessageWithLinks("Cherry picking one commit failed.");

            if (result.IsGitDisasterHappening != false)
            {
                FailureRemedy = new Remedy.MessageGitDisaster(this, message, result);
                return false;
            }

            if (result.ErrorConflicts)
            {
                FailureRemedy = new Remedy.ResolveCherryPickConflicts(this, message);
                return false;
            }

            if (!result.CherryPicked)
            {
                FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                return false;
            }

            return true;
        }
    }
}
