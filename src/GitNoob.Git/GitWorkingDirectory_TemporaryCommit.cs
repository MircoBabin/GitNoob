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
            var currentbranch = new Command.Branch.GetCurrentBranch(this);
            currentbranch.WaitFor();

            if (currentbranch.DetachedHead != false)
            {
                return new HasGitNoobTemporaryCommitResult()
                {
                    CurrentBranch = currentbranch.shortname,

                    ErrorDetachedHead = (currentbranch.DetachedHead != false),
                };
            }

            var baseCommit = new Command.Branch.FindCommonCommitOfTwoBranches(this, MainBranch, currentbranch.shortname);
            baseCommit.WaitFor();

            if (String.IsNullOrWhiteSpace(baseCommit.commitid))
            {
                return new HasGitNoobTemporaryCommitResult()
                {
                    CurrentBranch = currentbranch.shortname,

                    ErrorNoCommonCommitWithMainBranch = true,
                };
            }

            var commits = new Command.Branch.ListCommits(this, baseCommit.commitid, currentbranch.shortname);
            commits.WaitFor();

            uint count = 0;
            foreach (var commit in commits.result)
            {
                if (commit.Message.StartsWith(TemporaryCommitMessage))
                {
                    count++;
                }
            }

            return new HasGitNoobTemporaryCommitResult()
            {
                CurrentBranch = currentbranch.shortname,

                HasGitNoobTemporaryCommit = (count > 0),
                HasNoGitNoobTemporaryCommit = (count == 0),
                NumberOfGitNoobTemporaryCommits = count,
            };
        }

        private GitBranch CreateTemporaryBranchAndCheckout(string branchFromBranchNameOrCommitId)
        {
            string tempbranchname = "gitnoob-tempbranch-" + GitUtils.GenerateRandomSha1();

            var create = new Command.Branch.CreateBranch(this, tempbranchname, branchFromBranchNameOrCommitId, true);
            create.WaitFor();

            var tempbranch = new Command.Branch.GetCurrentBranch(this);
            tempbranch.WaitFor();
            if (tempbranch.shortname != tempbranchname)
            {
                return null;
            }

            return tempbranch.branch;
        }

        public RemoveLastTemporaryCommitOnCurrentBranchResult RemoveLastTemporaryCommitOnCurrentBranch()
        {
            var currentbranch = new Command.Branch.GetCurrentBranch(this);
            var changes = new Command.WorkingTree.HasChanges(this);
            var rebasing = new Command.WorkingTree.IsRebaseActive(this);
            var merging = new Command.WorkingTree.IsMergeActive(this);
            currentbranch.WaitFor();
            changes.WaitFor();
            rebasing.WaitFor();
            merging.WaitFor();

            if (changes.stagedUncommittedFiles != false || changes.workingtreeChanges != false || rebasing.result != false || merging.result != false || currentbranch.DetachedHead != false)
            {
                return new RemoveLastTemporaryCommitOnCurrentBranchResult()
                {
                    CurrentBranch = currentbranch.shortname,

                    ErrorDetachedHead = (currentbranch.DetachedHead != false),
                    ErrorStagedUncommittedFiles = (changes.stagedUncommittedFiles != false),
                    ErrorWorkingTreeChanges = (changes.workingtreeChanges != false),
                    ErrorRebaseInProgress = (rebasing.result != false),
                    ErrorMergeInProgress = (merging.result != false),
                };
            }

            var lastcommit = new Command.Branch.GetLastCommitOfBranch(this, currentbranch.shortname);
            lastcommit.WaitFor();
            if (lastcommit.commitmessage == null)
            {
                return new RemoveLastTemporaryCommitOnCurrentBranchResult()
                {
                    CurrentBranch = currentbranch.shortname,

                    NoCommitToRemove = true,
                };
            }

            if (!lastcommit.commitmessage.StartsWith(TemporaryCommitMessage))
            {
                return new RemoveLastTemporaryCommitOnCurrentBranchResult()
                {
                    CurrentBranch = currentbranch.shortname,

                    NoCommitToRemove = true,
                };
            }

            var unpack = new Command.Branch.ResetCurrentBranchToPreviousCommit(this, false);
            unpack.WaitFor();

            return new RemoveLastTemporaryCommitOnCurrentBranchResult()
            {
                CurrentBranch = currentbranch.shortname,

                Removed = true,
            };
        }
    }
}
