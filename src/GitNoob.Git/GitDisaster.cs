﻿using GitNoob.GitResult;

namespace GitNoob.Git
{
    public class GitDisaster
    {
        public static bool Check(GitWorkingDirectory gitworkingdirectory, BaseGitDisasterResult result, GitDisasterAllowed allowed = null)
        {
            var currentbranch = new Command.Branch.GetCurrentBranch(gitworkingdirectory);
            var changes = new Command.WorkingTree.HasChanges(gitworkingdirectory);

            Command.Branch.HasBranchUnpushedChanges unpushedCommitsOnMain = null;
            if (!string.IsNullOrWhiteSpace(gitworkingdirectory.RemoteUrl))
                unpushedCommitsOnMain = new Command.Branch.HasBranchUnpushedChanges(gitworkingdirectory, gitworkingdirectory.MainBranch, null);

            var rebasing = new Command.WorkingTree.IsRebaseActive(gitworkingdirectory);
            var merging = new Command.WorkingTree.IsMergeActive(gitworkingdirectory);
            var cherrypicking = new Command.WorkingTree.IsCherryPickActive(gitworkingdirectory);
            var reverting = new Command.WorkingTree.IsRevertActive(gitworkingdirectory);
            currentbranch.WaitFor();
            changes.WaitFor();

            if (!string.IsNullOrWhiteSpace(gitworkingdirectory.RemoteUrl))
                unpushedCommitsOnMain.WaitFor();

            rebasing.WaitFor();
            merging.WaitFor();
            cherrypicking.WaitFor();
            reverting.WaitFor();

            result.GitDisaster_CurrentGitBranch = currentbranch.branch;
            result.GitDisaster_CurrentBranchShortName = currentbranch.shortname;

            result.GitDisaster_DetachedHead = (currentbranch.DetachedHead != false);
            result.GitDisaster_StagedUncommittedFiles = (changes.stagedUncommittedFiles != false);
            result.GitDisaster_WorkingTreeChanges = (changes.workingtreeChanges != false);

            if (!string.IsNullOrWhiteSpace(gitworkingdirectory.RemoteUrl))
                result.GitDisaster_UnpushedCommitsOnMainBranch = (unpushedCommitsOnMain.result != false);
            else
                result.GitDisaster_UnpushedCommitsOnMainBranch = false;

            result.GitDisaster_RebaseInProgress = (rebasing.result != false);
            result.GitDisaster_MergeInProgress = (merging.result != false);
            result.GitDisaster_CherryPickInProgress = (cherrypicking.result != false);
            result.GitDisaster_RevertInProgress = (reverting.result != false);

            bool disaster = false;
            if (allowed == null)
                allowed = new GitDisasterAllowed();

            if (result.GitDisaster_DetachedHead != false && allowed.Allow_DetachedHead != true) disaster = true;
            if (result.GitDisaster_StagedUncommittedFiles != false && allowed.Allow_StagedUncommittedFiles != true) disaster = true;
            if (result.GitDisaster_WorkingTreeChanges != false && allowed.Allow_WorkingTreeChanges != true) disaster = true;
            if (result.GitDisaster_UnpushedCommitsOnMainBranch != false && allowed.Allow_UnpushedCommitsOnMainBranch != true) disaster = true;

            if (result.GitDisaster_RebaseInProgress != false && allowed.Allow_RebaseInProgress != true) disaster = true;
            if (result.GitDisaster_MergeInProgress != false && allowed.Allow_MergeInProgress != true) disaster = true;
            if (result.GitDisaster_CherryPickInProgress != false && allowed.Allow_CherryPickInProgress != true) disaster = true;
            if (result.GitDisaster_RevertInProgress != false && allowed.Allow_RevertInProgress != true) disaster = true;

            result.IsGitDisasterHappening = disaster;
            return disaster;
        }
    }
}
