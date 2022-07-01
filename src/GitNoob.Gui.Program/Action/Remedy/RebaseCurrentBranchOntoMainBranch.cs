using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class RebaseCurrentBranchOntoMainBranch : Remedy
    {
        public RebaseCurrentBranchOntoMainBranch(Step.Step Step, VisualizerMessageWithLinks Message,
            string MainBranch, string CurrentBranch) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("To incorporate the downloaded changes of the main branch \"" + MainBranch + "\", the current branch \"" + CurrentBranch + "\" must be rebased onto main branch.");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("This may lead to rebase conflicts. Upon conflicts, the rebase can be aborted or the conflicts can be resolved.");

            VisualizerMessageButtons = 
                new List<VisualizerMessageButton>()
                {
                    new VisualizerMessageButton("Don't rebase. The current branch \"" + CurrentBranch + "\" will not contain the downloaded changes.", (input) => {
                        Cancel();
                    }),
                    new VisualizerMessageButton("Start rebase. The current branch \"" + CurrentBranch + "\" will be updated to incorporate the changes of the main branch \"" + MainBranch + "\".", (input) => {
                        var step = new Step.RebaseCurrentBranchOntoMainBranch();
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });

                        Done();
                    }),
                };
        }
    }
}
