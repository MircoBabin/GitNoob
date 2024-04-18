using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class MoveChangesOnCurrentBranchToNewBranch : Step
    {
        private string _currentBranch;
        private string _newBranch;
        public MoveChangesOnCurrentBranchToNewBranch(string CurrentBranch, string NewBranch) : base()
        {
            _currentBranch = CurrentBranch;
            _newBranch = NewBranch;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - moving changes from current branch to new branch";

            var result = StepsExecutor.Config.Git.MoveUnpushedCommitsFromRemoteTrackingBranchToNewBranch(_currentBranch, _newBranch);

            var message = new VisualizerMessageWithLinks("Move changes is not possible.");

            if (result.IsGitDisasterHappening != false)
            {
                FailureRemedy = new Remedy.MessageGitDisaster(this, message, result);
                return false;
            }

            if (result.ErrorNotTrackingRemoteBranch)
            {
                FailureRemedy = new Remedy.MessageNotTrackingRemoteBranch(this, message, _currentBranch);
                return false;
            }

            StepsExecutor.CurrentBranchChangedTo(result.GitDisaster_CurrentBranchShortName);

            if (result.ErrorRenaming || result.ErrorRemovingRemote)
            {
                FailureRemedy = new Remedy.MessageMoveFailed(this, message, 
                    _currentBranch, _newBranch, result.GitDisaster_CurrentBranchShortName, 
                    result.ErrorRenaming, result.ErrorRemovingRemote);
                return false;
            }

            if (!result.Moved)
            {
                FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                return false;
            }

            if (result.GitDisaster_CurrentBranchShortName != _newBranch)
            {
                FailureRemedy = new Remedy.MessageUnexpectedCurrentBranch(this, message, result.GitDisaster_CurrentBranchShortName, _newBranch);
                return false;
            }

            return true;
        }
    }
}
