using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class RenameBranch : Step
    {
        private string _branchName;
        private string _newName;

        public RenameBranch(string BranchName, string NewName) : base()
        {
            _branchName = BranchName;
            _newName = NewName;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - renaming branch \"" + _branchName + "\" to \"" + _newName + "\"";

            {
                var result = StepsExecutor.Config.Git.RenameBranch(_branchName, _newName);

                var message = new VisualizerMessageWithLinks("Renaming branch failed.");

                if (result.IsGitDisasterHappening != false)
                {
                    FailureRemedy = new Remedy.MessageGitDisaster(this, message, result);
                    return false;
                }

                StepsExecutor.CurrentBranchChangedTo(null);
            }

            return true;
        }
    }
}
