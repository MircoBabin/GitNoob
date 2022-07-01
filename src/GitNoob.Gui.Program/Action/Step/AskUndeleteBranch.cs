using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Step
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
                //var step = new UndeleteBranch(false, branch);
                //StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });
            });
            return false;
        }
    }
}
