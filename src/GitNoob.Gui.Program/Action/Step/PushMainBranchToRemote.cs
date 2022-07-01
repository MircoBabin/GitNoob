using System;

namespace GitNoob.Gui.Program.Action.Step
{
    public class PushMainBranchToRemote : Step
    {
        private bool _onConflicts_ResetMainBranchToRemote_UsingCurrentBranchStored;
        public PushMainBranchToRemote(bool OnConflicts_ResetMainBranchToRemote_UsingCurrentBranchStored) : base()
        {
            _onConflicts_ResetMainBranchToRemote_UsingCurrentBranchStored = OnConflicts_ResetMainBranchToRemote_UsingCurrentBranchStored;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - pushing main branch \"" + StepsExecutor.Config.ProjectWorkingDirectory.Git.MainBranch + "\" to remote";

            var result = StepsExecutor.Config.Git.PushMainBranchToRemote();

            var message = new VisualizerMessageWithLinks("Pushing main branch to remote failed.");

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

            if (result.ErrorKeePassNotStarted)
            {
                FailureRemedy = new Remedy.MessageKeePassNotStarted(this, message);
                return false;
            }

            if (result.ErrorRemoteNotReachable)
            {
                FailureRemedy = new Remedy.MessageRemoteNotReachable(this, message, StepsExecutor.Config.Git.RemoteUrl);
                return false;
            }

            if (result.ErrorConflicts || result.ErrorStillUnpushedCommitsOnMainBranch)
            {
                if (_onConflicts_ResetMainBranchToRemote_UsingCurrentBranchStored)
                {
                    var resetResult = StepsExecutor.Config.Git.ResetMainBranchToRemote(StepsExecutor.CurrentBranchStored);

                    if (resetResult.ErrorRebaseInProgress || resetResult.ErrorMergeInProgress)
                    {
                        message.Append(Environment.NewLine);
                        message.Append("Resetting main branch to remote failed! A rebase/merge is in progress.");
                    }
                    else if (resetResult.ErrorWorkingTreeChanges || resetResult.ErrorStagedUncommittedFiles)
                    {
                        message.Append(Environment.NewLine);
                        message.Append("Resetting main branch to remote failed! There are working tree changes or staged uncommitted files.");
                    }
                    else if (resetResult.ErrorCurrentBranchCommitUnequalsMainBranchCommit)
                    {
                        message.Append(Environment.NewLine);
                        message.Append("Resetting main branch to remote failed! The current branch \"" + StepsExecutor.CurrentBranchStored + "\" is not even with the main branch. Commits would become unreachable.");
                    }
                    else if (!resetResult.Reset)
                    {
                        message.Append(Environment.NewLine);
                        message.Append("Resetting main branch to remote failed for an unknown reason!");
                    }
                }

                FailureRemedy = new Remedy.MessagePushConflicts(this, message, StepsExecutor.Config.ProjectWorkingDirectory.Git.MainBranch);
                return false;
            }

            if (!result.Pushed)
            {
                FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                return false;
            }

            return true;
        }
    }
}

