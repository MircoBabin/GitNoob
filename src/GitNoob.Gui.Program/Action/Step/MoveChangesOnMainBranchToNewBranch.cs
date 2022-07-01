namespace GitNoob.Gui.Program.Action.Step
{
    public class MoveChangesOnMainBranchToNewBranch : Step
    {
        private string _mainBranch;
        private string _newBranch;
        public MoveChangesOnMainBranchToNewBranch(string MainBranch, string NewBranch) : base()
        {
            _mainBranch = MainBranch;
            _newBranch = NewBranch;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - moving changes from main branch to new branch";

            var result = StepsExecutor.Config.Git.MoveUnpushedCommitsFromRemoteTrackingBranchToNewBranch(_mainBranch, _newBranch);

            var message = new VisualizerMessageWithLinks("Move changes is not possible.");

            if (result.ErrorNotTrackingRemoteBranch)
            {
                FailureRemedy = new Remedy.MessageNotTrackingRemoteBranch(this, message, _mainBranch);
                return false;
            }

            if (result.ErrorRenaming || result.ErrorRemovingRemote)
            {
                FailureRemedy = new Remedy.MessageMoveFailed(this, message, 
                    _mainBranch, _newBranch, null, 
                    result.ErrorRenaming, result.ErrorRemovingRemote);
                return false;
            }

            if (!result.Moved)
            {
                FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                return false;
            }

            return true;
        }
    }
}
