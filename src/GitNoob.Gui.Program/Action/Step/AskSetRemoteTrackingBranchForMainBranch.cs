using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Step
{
    public class AskSetRemoteTrackingBranchForMainBranch : Step
    {
        public AskSetRemoteTrackingBranchForMainBranch() : base() { }

        protected override bool run()
        {
            BusyMessage = "Busy - retrieving remotes";

            var result = StepsExecutor.Config.Git.RetrieveRemotes();

            var message = new VisualizerMessageWithLinks("Set remote branch for main branch \"" + MainBranch + "\" to:");

            //not really a failure, but a solution to choose a remote branch visually
            FailureRemedy = new Remedy.InputChooseRemote(this, message, result.Remotes, "Cancel, don't set remote branch", (name) =>
            {
                var step = new SetRemoteForBranch(MainBranch, name);
                StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });
            },
            "Create new remote \"origin\" - " + StepsExecutor.Config.ProjectWorkingDirectory.Git.RemoteUrl, () =>
            {
                var step = new SetRemoteForBranch(MainBranch, "origin", StepsExecutor.Config.ProjectWorkingDirectory.Git.RemoteUrl);
                StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });
            });
            return false;
        }
    }
}
