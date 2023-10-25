using GitNoob.GitResult;
using GitNoob.Utils;
using System;

namespace GitNoob.Git
{
    public partial class GitWorkingDirectory
    {
        public enum UnpackLastCommitType { All, OnlyUnpackTemporaryCommit }
        public UnpackLastCommitOnCurrentBranchResult UnpackLastCommitOnCurrentBranch(UnpackLastCommitType unpackType)
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
                return new UnpackLastCommitOnCurrentBranchResult()
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
                return new UnpackLastCommitOnCurrentBranchResult()
                {
                    CurrentBranch = currentbranch.shortname,

                    NoCommitToUnpack = true,
                };
            }

            if (unpackType == UnpackLastCommitType.OnlyUnpackTemporaryCommit &&
                !lastcommit.commitmessage.StartsWith(TemporaryCommitMessage))
            {
                return new UnpackLastCommitOnCurrentBranchResult()
                {
                    CurrentBranch = currentbranch.shortname,

                    NoCommitToUnpack = true,
                };
            }

            var unpack = new Command.Branch.ResetCurrentBranchToPreviousCommit(this, true);
            unpack.WaitFor();

            return new UnpackLastCommitOnCurrentBranchResult()
            {
                CurrentBranch = currentbranch.shortname,

                Unpacked = true,
            };
        }

        public MoveUnpushedCommitsFromRemoteTrackingBranchToNewBranchResult MoveUnpushedCommitsFromRemoteTrackingBranchToNewBranch(string remoteTrackingBranch, string newBranch)
        {
            var currentbranch = new Command.Branch.GetCurrentBranch(this);
            var remotebranch = new Command.Branch.GetRemoteBranch(this, remoteTrackingBranch);
            currentbranch.WaitFor();
            remotebranch.WaitFor();

            if (currentbranch.shortname == remoteTrackingBranch ||
                String.IsNullOrWhiteSpace(remotebranch.result))
            {
                return new MoveUnpushedCommitsFromRemoteTrackingBranchToNewBranchResult()
                {
                    ErrorBranchIsCurrent_UseMoveUnpushedCommitsAndWorkingTreeChangesFromCurrentRemoteTrackingBranchToNewBranch = currentbranch.shortname == remoteTrackingBranch,
                    ErrorNotTrackingRemoteBranch = String.IsNullOrWhiteSpace(remotebranch.result),
                };
            }

            var org_remote = remotebranch.result;

            var rename = new Command.Branch.RenameBranch(this, remoteTrackingBranch, newBranch);
            rename.WaitFor();

            if (rename.result != true)
            {
                return new MoveUnpushedCommitsFromRemoteTrackingBranchToNewBranchResult()
                {
                    ErrorRenaming = true,
                };
            }

            var removetracking = new Command.Branch.RemoveTrackingRemoteBranch(this, newBranch);
            removetracking.WaitFor();

            remotebranch = new Command.Branch.GetRemoteBranch(this, newBranch);
            remotebranch.WaitFor();

            if (!String.IsNullOrWhiteSpace(remotebranch.result))
            {
                return new MoveUnpushedCommitsFromRemoteTrackingBranchToNewBranchResult()
                {
                    ErrorRemovingRemote = true,
                };
            }

            //recreate tracking branch (might be the main branch)
            var create = new Command.Branch.CreateBranch(this, remoteTrackingBranch, org_remote, false);
            create.WaitFor();

            return new MoveUnpushedCommitsFromRemoteTrackingBranchToNewBranchResult()
            {
                Moved = true,
            };
        }

        public TouchCommitAndAuthorTimestampsOfCurrentBranchResult TouchCommitAndAuthorTimestampsOfCurrentBranch(DateTime toTime)
        {
            var currentbranch = new Command.Branch.GetCurrentBranch(this);
            var changes = new Command.WorkingTree.HasChanges(this);
            var rebasing = new Command.WorkingTree.IsRebaseActive(this);
            var merging = new Command.WorkingTree.IsMergeActive(this);
            var baseCommit = new Command.Branch.FindCommonCommitOfTwoBranches(this, MainBranch, currentbranch.shortname);
            currentbranch.WaitFor();
            changes.WaitFor();
            rebasing.WaitFor();
            merging.WaitFor();
            baseCommit.WaitFor();

            if (currentbranch.shortname == MainBranch ||
                changes.stagedUncommittedFiles != false ||
                changes.workingtreeChanges != false ||
                rebasing.result != false ||
                merging.result != false ||
                currentbranch.DetachedHead != false ||
                !String.IsNullOrWhiteSpace(currentbranch.branch.RemoteBranchFullName) ||
                String.IsNullOrWhiteSpace(baseCommit.commitid))
            {
                return new TouchCommitAndAuthorTimestampsOfCurrentBranchResult()
                {
                    CurrentBranch = currentbranch.shortname,

                    ErrorCurrentBranchIsMainBranch = (currentbranch.shortname == MainBranch),
                    ErrorDetachedHead = (currentbranch.DetachedHead != false),
                    ErrorStagedUncommittedFiles = (changes.stagedUncommittedFiles != false),
                    ErrorWorkingTreeChanges = (changes.workingtreeChanges != false),
                    ErrorRebaseInProgress = (rebasing.result != false),
                    ErrorMergeInProgress = (merging.result != false),
                    ErrorCurrentBranchIsTrackingRemoteBranch = !String.IsNullOrWhiteSpace(currentbranch.branch.RemoteBranchFullName),
                    ErrorNoCommonCommitWithMainBranch = String.IsNullOrWhiteSpace(baseCommit.commitid),
                };
            }

            //commits on current branch
            var commits = new Command.Branch.ListCommits(this, baseCommit.commitid, currentbranch.shortname);
            commits.WaitFor();
            if (commits.result == null || commits.result.Count == 0)
            {
                return new TouchCommitAndAuthorTimestampsOfCurrentBranchResult()
                {
                    CurrentBranch = currentbranch.shortname,

                    NoCommitsToTouch = true,
                };
            }
            commits.result.Reverse(); //Reverse so that commits are: oldest first, newest last

            //create a new temporary branch
            var tempbranch = CreateTemporaryBranchAndCheckout(baseCommit.commitid);
            if (tempbranch == null)
            {
                return new TouchCommitAndAuthorTimestampsOfCurrentBranchResult()
                {
                    CurrentBranch = currentbranch.shortname,

                    ErrorCreatingTemporaryBranch = true,
                };
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
                    var checkout = new Command.Branch.ChangeBranchTo(this, currentbranch.shortname);
                    checkout.WaitFor();

                    var deletetemp = new Command.Branch.DeleteBranch(this, tempbranch.ShortName, true);
                    deletetemp.WaitFor();

                    return new TouchCommitAndAuthorTimestampsOfCurrentBranchResult()
                    {
                        CurrentBranch = currentbranch.shortname,

                        ErrorCherryPickingCommit = true,
                    };

                }

                var touch = new Command.Branch.AmendLastCommit(this, toTime, toTime);
                touch.WaitFor();
            }

            //delete original current branch
            if (!CreateDeletedBranchUndoTag(currentbranch.shortname, MainBranch, "Touched commit and author timestamps to " + GitUtils.DateTimeToHumanString(toTime)))
            {
                var checkout = new Command.Branch.ChangeBranchTo(this, currentbranch.shortname);
                checkout.WaitFor();

                var deletetemp = new Command.Branch.DeleteBranch(this, tempbranch.ShortName, true);
                deletetemp.WaitFor();

                return new TouchCommitAndAuthorTimestampsOfCurrentBranchResult()
                {
                    CurrentBranch = currentbranch.shortname,

                    ErrorCreatingSafetyTag = true,
                };
            }

            var delete = new Command.Branch.DeleteBranch(this, currentbranch.shortname, true);
            delete.WaitFor();

            //Rename current checked out temporary branch to original current branch
            var rename = new Command.Branch.RenameBranch(this, tempbranch.ShortName, currentbranch.shortname);
            rename.WaitFor();

            return new TouchCommitAndAuthorTimestampsOfCurrentBranchResult()
            {
                CurrentBranch = currentbranch.shortname,

                Touched = true,
                NumberOfTouchedCommits = (uint)commits.result.Count,
            };
        }

        public StageAllChangesOnCurrentBranchResult StageAllChangesOnCurrentBranch()
        {
            {
                var currentbranch = new Command.Branch.GetCurrentBranch(this);
                var rebasing = new Command.WorkingTree.IsRebaseActive(this);
                var merging = new Command.WorkingTree.IsMergeActive(this);
                currentbranch.WaitFor();
                rebasing.WaitFor();
                merging.WaitFor();

                if (currentbranch.DetachedHead != false ||
                    rebasing.result != false ||
                    merging.result != false)
                {
                    return new StageAllChangesOnCurrentBranchResult()
                    {
                        ErrorDetachedHead = (currentbranch.DetachedHead != false),
                        ErrorRebaseInProgress = (rebasing.result != false),
                        ErrorMergeInProgress = (merging.result != false),
                    };
                }
            }

            {
                var commit = new Command.Branch.CommitAllChanges(this, null);
                commit.WaitFor();
            }

            return new StageAllChangesOnCurrentBranchResult()
            {
                Staged = true,
            };

        }

        public CommitAllChangesOnCurrentBranchResult CommitAllChangesOnCurrentBranch(string commitMessage)
        {
            if (String.IsNullOrWhiteSpace(commitMessage))
            {
                commitMessage = TemporaryCommitMessage;
            }

            {
                var currentbranch = new Command.Branch.GetCurrentBranch(this);
                var changes = new Command.WorkingTree.HasChanges(this);
                var rebasing = new Command.WorkingTree.IsRebaseActive(this);
                var merging = new Command.WorkingTree.IsMergeActive(this);
                currentbranch.WaitFor();
                changes.WaitFor();
                rebasing.WaitFor();
                merging.WaitFor();

                if (currentbranch.DetachedHead != false ||
                    changes.stagedUncommittedFiles != false ||
                    rebasing.result != false ||
                    merging.result != false)
                {
                    return new CommitAllChangesOnCurrentBranchResult()
                    {
                        ErrorDetachedHead = (currentbranch.DetachedHead != false),
                        ErrorStagedUncommittedFiles = (changes.stagedUncommittedFiles != false),
                        ErrorRebaseInProgress = (rebasing.result != false),
                        ErrorMergeInProgress = (merging.result != false),
                    };
                }
            }

            {
                var commit = new Command.Branch.CommitAllChanges(this, commitMessage);
                commit.WaitFor();
            }

            return new CommitAllChangesOnCurrentBranchResult()
            {
                Committed = true,
            };
        }
    }
}
