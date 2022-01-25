﻿namespace GitNoob.Gui.Program.Action.Step
{
    public class RebaseContinue : Step
    {
        public RebaseContinue() : base() { }

        protected override bool run()
        {
            BusyMessage = "Busy - continuing rebase";

            var result = StepsExecutor.Config.Git.RebaseContinue();

            var message = new MessageWithLinks("Continuing rebase failed.");

            if (result.ErrorNotRebasing)
            {
                return true;
            }

            if (result.ErrorConflicts)
            {
                FailureRemedy = new Remedy.ResolveRebaseConflicts(this, message, MainBranch, result.CurrentBranch);
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
