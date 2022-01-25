﻿namespace GitNoob.Gui.Program.Action.Step
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

            var result = StepsExecutor.Config.Git.MoveUnpushedCommitsAndWorkingTreeChangesFromCurrentRemoteTrackingBranchToNewBranch(_currentBranch, _newBranch);

            var message = new MessageWithLinks("Move changes is not possible.");

            if (result.ErrorDetachedHead)
            {
                FailureRemedy = new Remedy.MessageDetachedHead(this, message);
                return false;
            }

            if (result.ErrorNotTrackingRemoteBranch)
            {
                FailureRemedy = new Remedy.MessageNotTrackingRemoteBranch(this, message, _currentBranch);
                return false;
            }

            StepsExecutor.CurrentBranchChangedTo(result.CurrentBranch);

            if (result.ErrorRenaming || result.ErrorRemovingRemote)
            {
                FailureRemedy = new Remedy.MessageMoveFailed(this, message, 
                    _currentBranch, _newBranch, result.CurrentBranch, 
                    result.ErrorRenaming, result.ErrorRemovingRemote);
                return false;
            }

            if (!result.Moved)
            {
                FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                return false;
            }

            if (result.CurrentBranch != _newBranch)
            {
                FailureRemedy = new Remedy.MessageUnexpectedCurrentBranch(this, message, result.CurrentBranch, _newBranch);
                return false;
            }

            return true;
        }
    }
}
