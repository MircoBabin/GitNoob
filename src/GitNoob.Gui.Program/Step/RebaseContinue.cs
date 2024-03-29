﻿using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class RebaseContinue : Step
    {
        public RebaseContinue() : base() { }

        protected override bool run()
        {
            BusyMessage = "Busy - continuing rebase";

            var result = StepsExecutor.Config.Git.RebaseContinue();

            var message = new VisualizerMessageWithLinks("Continuing rebase failed.");

            if (result.IsGitDisasterHappening != false)
            {
                FailureRemedy = new Remedy.MessageGitDisaster(this, message, result);
                return false;
            }

            if (result.ErrorNotRebasing)
            {
                return true;
            }

            if (result.ErrorConflicts)
            {
                FailureRemedy = new Remedy.ResolveRebaseConflicts(this, message, MainBranch, result.GitDisaster_CurrentBranchShortName);
                return false;
            }

            if (!result.Rebased)
            {
                FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                return false;
            }

            return true;
        }
    }
}
