using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class CreateBranchOntoMainBranch : Step
    {
        private string _branchName;

        public CreateBranchOntoMainBranch(string BranchName) : base()
        {
            _branchName = BranchName;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - creating new branch \"" + _branchName + "\" onto main branch \"" + MainBranch + "\"";

            {
                var message = new VisualizerMessageWithLinks("Ensuring main branch existance failed.");

                var result = StepsExecutor.Config.Git.EnsureMainBranchExistance();
                if (!result.Exists)
                {
                    FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                    return false;
                }
            }

            {
                var result = StepsExecutor.Config.Git.CreateNewBranch(_branchName, MainBranch, true);

                var message = new VisualizerMessageWithLinks("Creating new branch failed.");

                if (result.IsGitDisasterHappening != false)
                {
                    FailureRemedy = new Remedy.MessageGitDisaster(this, message, result);
                    return false;
                }

                if (result.ErrorBranchAlreadyExists)
                {
                    FailureRemedy = new Remedy.MessageBranchAlreadyExists(this, message, _branchName);
                    return false;
                }

                if (result.Created)
                {
                    StepsExecutor.CurrentBranchChangedTo(result.CurrentBranch);
                }
                else
                {
                    StepsExecutor.CurrentBranchChangedTo(null);
                }
            }

            return true;
        }
    }
}
