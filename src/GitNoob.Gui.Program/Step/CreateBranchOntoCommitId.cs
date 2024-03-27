using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class CreateBranchOntoCommitId : Step
    {
        private string _branchName;
        private string _onCommitId;

        public CreateBranchOntoCommitId(string BranchName, string OnCommitId) : base()
        {
            _branchName = BranchName;
            _onCommitId = OnCommitId;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - creating new branch \"" + _branchName + "\" onto commit-id \"" + _onCommitId + "\"";

            {
                var result = StepsExecutor.Config.Git.CreateNewBranch(_branchName, _onCommitId, true);

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
