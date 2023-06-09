using GitNoob.Gui.Visualizer;
using System;

namespace GitNoob.Gui.Program.Step
{
    public class RebuildCacheOnMainBranchAndPushToRemote : Step
    {
        private bool _useCurrentBranchStored;
        public RebuildCacheOnMainBranchAndPushToRemote(bool UseCurrentBranchStored) : base()
        {
            _useCurrentBranchStored = UseCurrentBranchStored;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - rebuilding cache";

            string CurrentBranch; //Should be even with main branch
            if (_useCurrentBranchStored)
            {
                CurrentBranch = StepsExecutor.CurrentBranchStored; 
            }
            else
            {
                throw new Exception("Unknown current branch");
            }

            var result = StepsExecutor.Config.Git.BuildCacheAndCommitOnMainBranch(StepsExecutor.Config.Executor, "Rebuild cache");

            var message = new VisualizerMessageWithLinks("Rebuilding cache failed.");

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

            if (result.ErrorUnpushedCommitsOnMainBranch)
            {
                FailureRemedy = new Remedy.MessageUnpushedCommits(this, message, MainBranch);
                return false;
            }

            if (result.ErrorChangingToMainBranch)
            {
                FailureRemedy = new Remedy.MessageChangingBranchFailed(this, message, MainBranch);
                return false;
            }

            if (result.ErrorBuildingCache)
            {
                StepsExecutor.Config.Git.DeleteAllWorkingTreeChangesAndStagedUncommittedFiles_ResetMainBranchToRemote_CheckoutBranch(CurrentBranch);

                FailureRemedy = new Remedy.MessageBuildingCacheFailed(this, message, result.BuildCache);
                return false;
            }

            if (result.NotUpdatedBecauseNothingChanged)
            {
                return true;
            }

            if (result.ErrorCommittingChanges)
            {
                StepsExecutor.Config.Git.DeleteAllWorkingTreeChangesAndStagedUncommittedFiles_ResetMainBranchToRemote_CheckoutBranch(CurrentBranch);

                FailureRemedy = new Remedy.MessageCommittingChangesFailed(this, message);
                return false;
            }

            if (!result.Updated)
            {
                FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                return false;
            }

            //push
            BusyMessage = "Busy - pushing cache to remote";

            var pushResult = StepsExecutor.Config.Git.PushMainBranchToRemote();

            message = new VisualizerMessageWithLinks("Pushing cache to remote failed.");

            if (pushResult.ErrorRebaseInProgress || pushResult.ErrorMergeInProgress)
            {
                FailureRemedy = new Remedy.MessageRebasingOrMerging(this, message, pushResult.ErrorRebaseInProgress, pushResult.ErrorMergeInProgress);
                return false;
            }

            if (pushResult.ErrorWorkingTreeChanges || pushResult.ErrorStagedUncommittedFiles)
            {
                FailureRemedy = new Remedy.MessageChanges(this, message, pushResult.ErrorWorkingTreeChanges, pushResult.ErrorStagedUncommittedFiles);
                return false;
            }

            if (pushResult.ErrorKeePassNotStarted)
            {
                StepsExecutor.Config.Git.DeleteAllWorkingTreeChangesAndStagedUncommittedFiles_ResetMainBranchToRemote_CheckoutBranch(CurrentBranch);

                FailureRemedy = new Remedy.MessageKeePassNotStarted(this, message);
                return false;
            }

            if (pushResult.ErrorRemoteNotReachable)
            {
                StepsExecutor.Config.Git.DeleteAllWorkingTreeChangesAndStagedUncommittedFiles_ResetMainBranchToRemote_CheckoutBranch(CurrentBranch);

                FailureRemedy = new Remedy.MessageRemoteNotReachable(this, message, StepsExecutor.Config.Git.RemoteUrl);
                return false;
            }

            if (pushResult.ErrorConflicts || pushResult.ErrorStillUnpushedCommitsOnMainBranch)
            {
                StepsExecutor.Config.Git.DeleteAllWorkingTreeChangesAndStagedUncommittedFiles_ResetMainBranchToRemote_CheckoutBranch(CurrentBranch);

                FailureRemedy = new Remedy.MessagePushConflicts(this, message, StepsExecutor.Config.ProjectWorkingDirectory.Git.MainBranch);
                return false;
            }

            if (!pushResult.Pushed)
            {
                StepsExecutor.Config.Git.DeleteAllWorkingTreeChangesAndStagedUncommittedFiles_ResetMainBranchToRemote_CheckoutBranch(CurrentBranch);

                FailureRemedy = new Remedy.MessageUnknownResult(this, message, pushResult);
                return false;
            }

            return true;
        }
    }
}
