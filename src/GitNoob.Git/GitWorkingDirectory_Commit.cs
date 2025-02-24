using GitNoob.GitResult;
using GitNoob.Utils;
using System;

namespace GitNoob.Git
{
    public partial class GitWorkingDirectory
    {
        public enum UnpackLastCommitType { All, OnlyUnpackTemporaryCommit }
        public UnpackLastCommitOnCurrentBranchResult UnpackLastCommitOnCurrentBranch(UnpackLastCommitType unpackType, string createUndeleteTagMessage)
        {
            var result = new UnpackLastCommitOnCurrentBranchResult();
            if (GitDisaster.Check(this, result, new GitDisasterAllowed()
            {
                Allow_WorkingTreeChanges = true,
                Allow_StagedUncommittedFiles = true,
                Allow_UnpushedCommitsOnMainBranch = true,
            }))
                return result;

            bool workingtreeChanges =
                (result.GitDisaster_WorkingTreeChanges != false) ||
                (result.GitDisaster_StagedUncommittedFiles != false);

            var lastcommit = new Command.Branch.GetLastCommitOfBranch(this, result.GitDisaster_CurrentBranchShortName);
            lastcommit.WaitFor();
            if (lastcommit.commitmessage == null)
            {
                result.NoCommitToUnpack = true;

                return result;
            }

            if (unpackType == UnpackLastCommitType.OnlyUnpackTemporaryCommit &&
                !lastcommit.commitmessage.StartsWith(TemporaryCommitMessage))
            {
                result.NoCommitToUnpack = true;

                return result;
            }

            if (!string.IsNullOrWhiteSpace(createUndeleteTagMessage))
            {
                if (!CreateDeletedBranchUndoTag(result.GitDisaster_CurrentBranchShortName, MainBranch,
                    createUndeleteTagMessage.Replace("<<lastcommit-message>>", lastcommit.commitmessage)))
                {
                    result.ErrorCreatingSafetyTag = true;
                    return result;
                }
            }

            if (workingtreeChanges)
            {
                var now = DateTime.Now;
                var amend = new Command.Branch.AmendLastCommit(this, true, now, now, "Unpack - amend last commit with working tree changes: " + lastcommit.commitmessage);
                amend.WaitFor();

                var changes = new Command.WorkingTree.HasChanges(this);
                changes.WaitFor();

                if (changes.workingtreeChanges != false || changes.stagedUncommittedFiles != false)
                {
                    result.ErrorAmendingLastCommitWithWorkingTreeChanges = true;

                    return result;
                }

                result.GitDisaster_StagedUncommittedFiles = false;
                result.GitDisaster_WorkingTreeChanges = false;
            }

            var unpack = new Command.Branch.ResetCurrentBranchToPreviousCommit(this, true);
            unpack.WaitFor();

            result.Unpacked = true;
            return result;
        }

        public TouchCommitAndAuthorTimestampsOfCurrentBranchResult TouchCommitAndAuthorTimestampsOfCurrentBranch(DateTime toTime)
        {
            var result = new TouchCommitAndAuthorTimestampsOfCurrentBranchResult();
            if (GitDisaster.Check(this, result))
                return result;

            if (!String.IsNullOrWhiteSpace(result.GitDisaster_CurrentGitBranch.RemoteBranchFullName))
            {
                result.ErrorCurrentBranchIsTrackingRemoteBranch = true;

                return result;
            }

            var baseCommit = new Command.Branch.FindCommonCommitOfTwoBranches(this, MainBranch, result.GitDisaster_CurrentBranchShortName);
            baseCommit.WaitFor();

            if (String.IsNullOrWhiteSpace(baseCommit.commitid))
            {
                result.ErrorNoCommonCommitWithMainBranch = true;

                return result;
            }

            //commits on current branch
            var commits = new Command.Branch.ListCommits(this, baseCommit.commitid, result.GitDisaster_CurrentBranchShortName);
            commits.WaitFor();
            if (commits.result == null || commits.result.Count == 0)
            {
                result.NoCommitsToTouch = true;

                return result;
            }
            commits.result.Reverse(); //Reverse so that commits are: oldest first, newest last

            //create a new temporary branch
            var tempbranch = CreateTemporaryBranchAndCheckout(baseCommit.commitid);
            if (tempbranch == null)
            {
                result.ErrorCreatingTemporaryBranch = true;

                return result;
            }

            //cherry pick each commit and amend commit & author timestamp
            foreach (var commit in commits.result)
            {
                var cherrypick = new Command.Branch.CherryPickOneCommit(this, commit.CommitId);
                cherrypick.WaitFor();

                var lastcommit = new Command.Branch.GetLastCommitOfBranch(this, tempbranch.ShortName);
                lastcommit.WaitFor();

                var same = new Command.Repository.CommitsAreTheSame(this, commit.CommitId, lastcommit.commitid);
                same.WaitFor();

                if (same.result != true)
                {
                    var checkout = new Command.Branch.ChangeBranchTo(this, result.GitDisaster_CurrentBranchShortName);
                    checkout.WaitFor();

                    var deletetemp = new Command.Branch.DeleteBranch(this, tempbranch.ShortName, true);
                    deletetemp.WaitFor();

                    result.ErrorCherryPickingCommit = true;
                    return result;
                }

                var touch = new Command.Branch.AmendLastCommit(this, false, toTime, toTime, null);
                touch.WaitFor();
            }

            //delete original current branch
            if (!CreateDeletedBranchUndoTag(result.GitDisaster_CurrentBranchShortName, MainBranch, "Touched commit and author timestamps to " + GitUtils.DateTimeToHumanString(toTime)))
            {
                var checkout = new Command.Branch.ChangeBranchTo(this, result.GitDisaster_CurrentBranchShortName);
                checkout.WaitFor();

                var deletetemp = new Command.Branch.DeleteBranch(this, tempbranch.ShortName, true);
                deletetemp.WaitFor();

                result.ErrorCreatingSafetyTag = true;
                return result;
            }

            var delete = new Command.Branch.DeleteBranch(this, result.GitDisaster_CurrentBranchShortName, true);
            delete.WaitFor();

            //Rename current checked out temporary branch to original current branch
            var rename = new Command.Branch.RenameBranch(this, tempbranch.ShortName, result.GitDisaster_CurrentBranchShortName);
            rename.WaitFor();

            result.Touched = true;
            result.NumberOfTouchedCommits = (uint)commits.result.Count;
            return result;
        }

        public StageAllChangesOnCurrentBranchResult StageAllChangesOnCurrentBranch()
        {
            var result = new StageAllChangesOnCurrentBranchResult();
            if (GitDisaster.Check(this, result, new GitDisasterAllowed()
            {
                Allow_UnpushedCommitsOnMainBranch = true,
                Allow_WorkingTreeChanges = true,
                Allow_StagedUncommittedFiles = true,
            }))
                return result;


            {
                var commit = new Command.Branch.CommitAllChanges(this, null);
                commit.WaitFor();
            }

            result.Staged = true;
            return result;
        }

        public CommitAllChangesOnCurrentBranchResult CommitAllChangesOnCurrentBranch(string commitMessage)
        {
            var result = new CommitAllChangesOnCurrentBranchResult();
            if (GitDisaster.Check(this, result, new GitDisasterAllowed()
            {
                Allow_UnpushedCommitsOnMainBranch = true,
                Allow_WorkingTreeChanges = true,
                Allow_StagedUncommittedFiles = true,
            }))
                return result;

            if (String.IsNullOrWhiteSpace(commitMessage))
            {
                commitMessage = TemporaryCommitMessage;
            }

            {
                var commit = new Command.Branch.CommitAllChanges(this, commitMessage);
                commit.WaitFor();
            }

            result.Committed = true;
            return result;
        }
    }
}
