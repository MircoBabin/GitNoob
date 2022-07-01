using System;

namespace GitNoob.Gui.Program.Action.Step
{
    public class CheckoutBranch : Step
    {
        private bool _useCurrentBranchStored;
        private string _branchName;

        public CheckoutBranch(bool UseCurrentBranchStored, string BranchName) : base()
        {
            if (UseCurrentBranchStored)
            {
                _useCurrentBranchStored = true;
                _branchName = string.Empty;
            }
            else
            {
                _useCurrentBranchStored = false;
                _branchName = BranchName;
            }
        }

        protected override bool run()
        {
            if (_useCurrentBranchStored)
            {
                _branchName = StepsExecutor.CurrentBranchStored;
            }

            BusyMessage = "Busy - changing to branch \"" + _branchName + "\"";

            {
                var result = StepsExecutor.Config.Git.ChangeCurrentBranchTo(_branchName);

                var message = new VisualizerMessageWithLinks("Changing branch failed.");

                if (result.ErrorRebaseInProgress || result.ErrorMergeInProgress)
                {
                    FailureRemedy = new Remedy.MessageRebasingOrMerging(this, message, result.ErrorRebaseInProgress, result.ErrorMergeInProgress);
                    return false;
                }

                if (result.ErrorWorkingTreeChanges || result.ErrorStagedUncommittedFiles)
                {
                    FailureRemedy = new Remedy.MessageChanges(this, message, result.ErrorWorkingTreeChanges, result.ErrorStagedUncommittedFiles);
                    return false;
                }

                if (result.Changed)
                {
                    StepsExecutor.CurrentBranchChangedTo(result.CurrentBranch);
                }
                else
                {
                    StepsExecutor.CurrentBranchChangedTo(null);
                }
            }

            {
                var message = new VisualizerMessageWithLinks("Changing branch succeeded.");
                message.Append(Environment.NewLine);
                message.Append(Environment.NewLine);
                message.Append("Unpacking the temporary commit failed.");

                var result = StepsExecutor.Config.Git.UnpackLastCommitOnCurrentBranch(Git.GitWorkingDirectory.UnpackLastCommitType.OnlyUnpackTemporaryCommit);
                if (result.ErrorRebaseInProgress || result.ErrorMergeInProgress)
                {
                    FailureRemedy = new Remedy.MessageRebasingOrMerging(this, message, result.ErrorRebaseInProgress, result.ErrorMergeInProgress);
                    return false;
                }

                if (result.ErrorDetachedHead)
                {
                    FailureRemedy = new Remedy.MessageDetachedHead(this, message);
                    return false;
                }

                if (result.ErrorWorkingTreeChanges || result.ErrorStagedUncommittedFiles)
                {
                    FailureRemedy = new Remedy.MessageChanges(this, message, result.ErrorWorkingTreeChanges, result.ErrorStagedUncommittedFiles);
                    return false;
                }

                if (!result.NoCommitToUnpack && !result.Unpacked)
                {
                    FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                    return false;
                }
            }

            return true;
        }
    }
}
