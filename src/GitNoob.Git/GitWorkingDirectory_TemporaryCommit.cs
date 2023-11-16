using GitNoob.GitResult;
using GitNoob.Utils;
using System;

namespace GitNoob.Git
{
    public partial class GitWorkingDirectory
    {
        public const string TemporaryCommitMessage = "[GitNoob][Temporary Commit]";

        public HasGitNoobTemporaryCommitResult HasCurrentBranchUntilMainBranchGitNoobTemporaryCommits()
        {
            var result = new HasGitNoobTemporaryCommitResult();
            if (GitDisaster.Check(this, result, new GitDisasterAllowed()
            {
                Allow_UnpushedCommitsOnMainBranch = true,
                Allow_StagedUncommittedFiles = true,
                Allow_WorkingTreeChanges = true,

                Allow_CherryPickInProgress = true,
                Allow_MergeInProgress = true,
                Allow_RebaseInProgress = true,
                Allow_RevertInProgress = true,

                //Only disallow DetachedHead
            }))
                return result;

            var baseCommit = new Command.Branch.FindCommonCommitOfTwoBranches(this, MainBranch, result.GitDisaster_CurrentBranchShortName);
            baseCommit.WaitFor();

            if (String.IsNullOrWhiteSpace(baseCommit.commitid))
            {
                result.ErrorNoCommonCommitWithMainBranch = true;

                return result;
            }

            var commits = new Command.Branch.ListCommits(this, baseCommit.commitid, result.GitDisaster_CurrentBranchShortName);
            commits.WaitFor();

            uint count = 0;
            foreach (var commit in commits.result)
            {
                if (commit.Message.StartsWith(TemporaryCommitMessage))
                {
                    count++;
                }
            }

            result.HasGitNoobTemporaryCommit = (count > 0);
            result.HasNoGitNoobTemporaryCommit = (count == 0);
            result.NumberOfGitNoobTemporaryCommits = count;
            return result;
        }

        public RemoveLastTemporaryCommitOnCurrentBranchResult RemoveLastTemporaryCommitOnCurrentBranch()
        {
            var result = new RemoveLastTemporaryCommitOnCurrentBranchResult();
            if (GitDisaster.Check(this, result, new GitDisasterAllowed()
            {
                Allow_UnpushedCommitsOnMainBranch = true,
            }))
                return result;

            var lastcommit = new Command.Branch.GetLastCommitOfBranch(this, result.GitDisaster_CurrentBranchShortName);
            lastcommit.WaitFor();
            if (lastcommit.commitmessage == null)
            {
                result.NoCommitToRemove = true;

                return result;
            }

            if (!lastcommit.commitmessage.StartsWith(TemporaryCommitMessage))
            {
                result.NoCommitToRemove = true;

                return result;
            }

            var unpack = new Command.Branch.ResetCurrentBranchToPreviousCommit(this, false);
            unpack.WaitFor();

            result.Removed = true;
            return result;
        }
    }
}
