using GitNoob.Gui.Visualizer;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Step
{
    public class AskChangeBranch : Step
    {
        public AskChangeBranch() : base() { }

        protected override bool run()
        {
            BusyMessage = "Busy - retrieving branches";

            var result = StepsExecutor.Config.Git.RetrieveBranches();

            var message = new VisualizerMessageWithLinks("Change branch is not possible.");

            if (result.Branches.Count == 0)
            {
                FailureRemedy = new Remedy.MessageNoBranches(this, message);
                return false;
            }

            //not really a failure, but a solution to choose a branch visually
            message = new VisualizerMessageWithLinks("Change branch to:");
            string currentBranch = result.CurrentBranch;
            string renameBranch = "Rename current branch \"" + currentBranch + "\".";
            string newBranch = "Create a new branch based on the main branch \"" + MainBranch + "\".";
            string newBranchOnCommit = "Create a new branch on a specific commit-id.";
            string deleteBranch = "Delete current branch \"" + currentBranch + "\".";
            FailureRemedy = new Remedy.InputChooseBranch(this, message, result.Branches, "Cancel, don't change branch", 
                (!result.CurrentBranchIsTrackingRemoteBranch && !result.DetachedHead_NotOnBranch ? renameBranch : null), currentBranch,
                newBranch, MainBranch,
                newBranchOnCommit,
                deleteBranch, 
                (name) => {
                var step = new CheckoutBranch(false, name);
                StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });
            });
            return false;
        }
    }
}
