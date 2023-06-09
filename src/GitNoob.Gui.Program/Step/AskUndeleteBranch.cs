using GitNoob.Gui.Visualizer;
using GitNoob.Utils;
using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Step
{
    public class AskUndeleteBranch : Step
    {
        public AskUndeleteBranch() : base() { }

        protected override bool run()
        {
            BusyMessage = "Busy - retrieving deleted branches";

            var result = StepsExecutor.Config.Git.RetrieveDeletedBranches();

            var message = new VisualizerMessageWithLinks("Undelete branch is not possible.");

            if (result.DeletedBranches.Count == 0)
            {
                FailureRemedy = new Remedy.MessageNoDeletedBranches(this, message);
                return false;
            }

            //not really a failure, but a solution to choose a deleted branch visually
            message = new VisualizerMessageWithLinks("Undelete branch:");
            FailureRemedy = new Remedy.InputChooseDeletedBranch(this, message, result.DeletedBranches, "Cancel",
                MainBranch,
                (branch) => {
                    var msg = new VisualizerMessageWithLinks("Undelete branch " + branch.BranchName + ", deleted on " + GitUtils.DateTimeToHumanString(branch.DeletionTime) + ".");
                    if (!string.IsNullOrWhiteSpace(branch.Message))
                    {
                        msg.Append(Environment.NewLine);
                        msg.Append(branch.Message);
                    }

                    var remedy = new Remedy.InputNewBranchName(this, msg, "Undelete branch. Create new branch on last commit of deleted branch.", (input) => {
                        var undeleteStep = new UndeleteBranch(branch, input);
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { undeleteStep });
                    });
                    StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { remedy });
                });
            return false;
        }
    }
}
