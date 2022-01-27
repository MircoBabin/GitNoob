﻿using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Step
{
    public class AskChangeBranch : Step
    {
        public AskChangeBranch() : base() { }

        protected override bool run()
        {
            BusyMessage = "Busy - retrieving branches";

            var result = StepsExecutor.Config.Git.RetrieveBranches();

            var message = new MessageWithLinks("Change branch is not possible.");

            if (result.Branches.Count == 0)
            {
                FailureRemedy = new Remedy.MessageNoBranches(this, message);
                return false;
            }

            //not really a failure, but a solution to choose a branch visually
            message = new MessageWithLinks("Change branch to:");
            string newBranch = "Create a new branch based on the main branch \"" + MainBranch + "\".";
            FailureRemedy = new Remedy.InputChooseBranch(this, message, result.Branches, "Cancel, don't change branch", newBranch, MainBranch, (name) => {
                var step = new CheckoutBranch(false, name);
                StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });
            });
            return false;
        }
    }
}