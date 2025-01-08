using GitNoob.Gui.Visualizer;
using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Remedy
{
    public class RebaseCurrentBranchWithChangesOntoMainBranch : Remedy
    {
        public RebaseCurrentBranchWithChangesOntoMainBranch(Step.Step Step, VisualizerMessageWithLinks Message,
            string MainBranch, string CurrentBranch) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("The current branch \"" + CurrentBranch + "\" has uncommitted changes." + Environment.NewLine);
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("To incorporate the downloaded changes of the main branch \"" + MainBranch + "\", the current branch \"" + CurrentBranch + "\" must be rebased onto main branch.");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("This may lead to rebase conflicts. Upon conflicts, the rebase can be aborted or the conflicts can be resolved.");
            VisualizerMessageText.Append(Environment.NewLine);

            VisualizerMessageButtons = 
                new List<VisualizerMessageButton>()
                {
                    new VisualizerMessageButton("Don't rebase. The current branch \"" + CurrentBranch + "\" will not contain the downloaded changes.", (input) => {
                        Cancel();
                    }),
                    new VisualizerMessageButton("Commit current changes into a temporary commit and start the rebase. The current branch \"" + CurrentBranch + "\" will be updated to incorporate the changes of the main branch \"" + MainBranch + "\".", (input) => {
                        var step1 = new Step.TemporaryCommitChangesOnCurrentBranch();
                        var step2 = new Step.RebaseCurrentBranchOntoMainBranch("Safety - get latest - before rebasing with changes onto mainbranch.");
                        var step3 = new Step.UnpackLastCommitOnCurrentBranch(true);
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step1, step2, step3 });

                        Done();
                    }),
                };
        }
    }
}
