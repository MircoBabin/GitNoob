using GitNoob.GitResult;
using GitNoob.Utils;
using System;

namespace GitNoob.Git
{
    public partial class GitWorkingDirectory
    {
        public GitBranch RetrieveMainBranch()
        {
            var mainbranch = new Command.Branch.ListBranches(this, true, MainBranch);
            mainbranch.WaitFor();

            if (mainbranch.result.Count != 1)
            {
                return null;
            }

            return mainbranch.result[0];
        }

        public EnsureMainBranchExistanceResult EnsureMainBranchExistance()
        {
            var list = new Command.Branch.ListBranches(this, true);
            list.WaitFor();

            GitBranch remote = null;
            foreach (var branch in list.result)
            {
                switch (branch.Type)
                {
                    case GitBranch.BranchType.LocalTrackingRemoteBranch:
                        if (branch.ShortName == MainBranch)
                        {
                            return new EnsureMainBranchExistanceResult()
                            {
                                Exists = true,
                            };
                        }
                        break;

                    case GitBranch.BranchType.UntrackedRemoteBranch:
                        if (remote == null)
                        {
                            var parts = branch.FullName.Split('/');
                            if (parts[parts.Length - 1] == MainBranch)
                            {
                                remote = branch;
                            }
                        }
                        break;
                }
            }

            if (remote == null)
            {
                return new EnsureMainBranchExistanceResult()
                {
                    ErrorRemoteBranchNotFound = true,
                };
            }

            var create = new Command.Branch.CreateBranch(this, MainBranch, remote.FullName, false);
            create.WaitFor();

            list = new Command.Branch.ListBranches(this, false, MainBranch);
            list.WaitFor();

            if (list.result.Count != 1)
            {
                return new EnsureMainBranchExistanceResult()
                {
                    ErrorCreatingMainBranch = true,
                };
            }

            if (list.result[0].Type != GitBranch.BranchType.LocalTrackingRemoteBranch)
            {
                return new EnsureMainBranchExistanceResult()
                {
                    ErrorCreatingMainBranch = true,
                };
            }

            return new EnsureMainBranchExistanceResult()
            {
                Exists = true,
            };
        }

        public ResetMainBranchToRemoteResult ResetMainBranchToRemote(string currentBranch)
        {
            if (currentBranch == MainBranch)
            {
                return new ResetMainBranchToRemoteResult()
                {
                    ErrorCurrentBranchIsMainBranch = true,
                };
            }

            var currentCommit = new Command.Branch.GetLastCommitOfBranch(this, currentBranch);
            var mainCommit = new Command.Branch.GetLastCommitOfBranch(this, MainBranch);
            var changes = new Command.WorkingTree.HasChanges(this);
            var rebasing = new Command.WorkingTree.IsRebaseActive(this);
            var merging = new Command.WorkingTree.IsMergeActive(this);
            changes.WaitFor();
            rebasing.WaitFor();
            merging.WaitFor();
            currentCommit.WaitFor();
            mainCommit.WaitFor();

            if (changes.stagedUncommittedFiles != false || changes.workingtreeChanges != false || rebasing.result != false || merging.result != false)
            {
                return new ResetMainBranchToRemoteResult()
                {
                    ErrorStagedUncommittedFiles = (changes.stagedUncommittedFiles != false),
                    ErrorWorkingTreeChanges = (changes.workingtreeChanges != false),
                    ErrorRebaseInProgress = (rebasing.result != false),
                    ErrorMergeInProgress = (merging.result != false),
                };
            }


            if (string.IsNullOrWhiteSpace(currentCommit.commitid) || string.IsNullOrWhiteSpace(mainCommit.commitid))
            {
                return new ResetMainBranchToRemoteResult()
                {
                    ErrorCurrentBranchCommitUnequalsMainBranchCommit = true,
                };
            }

            if (currentCommit.commitid != mainCommit.commitid)
            {
                return new ResetMainBranchToRemoteResult()
                {
                    ErrorCurrentBranchCommitUnequalsMainBranchCommit = true,
                };
            }

            {
                //forced delete local main branch
                var delete = new Command.Branch.DeleteBranch(this, MainBranch, true);
                delete.WaitFor();

                //recreate automatically as a tracking branch by git checkout
                var checkout = new Command.Branch.ChangeBranchTo(this, currentBranch);
                checkout.WaitFor();
            }

            return new ResetMainBranchToRemoteResult()
            {
                Reset = true,
            };
        }

        public RemotesResult RetrieveRemotes()
        {
            var result = new RemotesResult();

            var command = new Command.Remote.ListRemotes(this);
            command.WaitFor();


            if (command.result != null)
            {
                foreach (var keypair in command.result)
                {
                    result.Remotes.Add(keypair.Value);
                }
            }

            return result;
        }

        public SetRemoteForBranchResult SetRemoteForBranch(string branch, string remoteName, string remoteUrl = null)
        {
            if (!string.IsNullOrEmpty(remoteUrl))
            {
                var change = new Command.Remote.ChangeUrl(this, remoteName, remoteUrl);
                change.WaitFor();

                var list = new Command.Remote.ListRemotes(this);
                list.WaitFor();

                if (list.result != null)
                {
                    bool found = false;
                    foreach (var keypair in list.result)
                    {
                        if (keypair.Value.RemoteName == remoteName)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        return new SetRemoteForBranchResult()
                        {
                            ErrorSettingRemoteUrl = true,
                        };
                    }
                }
            }

            {
                var command = new Command.Branch.SetTrackingRemoteBranch(this, branch, remoteName, branch);
                command.WaitFor();

                var branchremote = new Command.Branch.GetRemoteBranch(this, branch);
                branchremote.WaitFor();

                if (branchremote.result != (remoteName + "/" + branch))
                {
                    return new SetRemoteForBranchResult()
                    {
                        ErrorSettingRemoteForBranch = true,
                    };
                }
            }

            return new SetRemoteForBranchResult()
            {
                RemoteSet = true,
            };
        }

        public BranchesResult RetrieveBranches()
        {
            var result = new BranchesResult();

            var command = new Command.Branch.ListBranches(this, true);

            var currentbranch = new Command.Branch.GetCurrentBranch(this);
            currentbranch.WaitFor();
            result.DetachedHead_NotOnBranch = (currentbranch.DetachedHead == true);
            result.CurrentBranch = currentbranch.shortname;
            if (currentbranch.DetachedHead == false)
            {
                var remotebranch = new Command.Branch.GetRemoteBranch(this, currentbranch.shortname);
                remotebranch.WaitFor();
                result.CurrentBranchIsTrackingRemoteBranch = !string.IsNullOrWhiteSpace(remotebranch.result);
            }

            command.WaitFor();
            if (command.result != null)
            {
                foreach (var branch in command.result)
                {
                    result.Branches.Add(branch.ShortName);
                }
            }

            return result;
        }

        public RenameBranchResult RenameBranch(string branchname, string newname)
        {
            var rebasing = new Command.WorkingTree.IsRebaseActive(this);
            var merging = new Command.WorkingTree.IsMergeActive(this);

            rebasing.WaitFor();
            merging.WaitFor();

            if (rebasing.result != false || merging.result != false)
            {
                return new RenameBranchResult()
                {
                    ErrorRebaseInProgress = (rebasing.result != false),
                    ErrorMergeInProgress = (merging.result != false),
                };
            }

            var rename = new Command.Branch.RenameBranch(this, branchname, newname);
            rename.WaitFor();

            if (rename.result != true)
            {
                return new RenameBranchResult()
                {
                    ErrorRenaming = true,
                };
            }

            return new RenameBranchResult()
            {
                Renamed = true,
            };
        }

        public CreateNewBranchResult CreateNewBranch(string branchname, string branchFromBranchName, bool checkoutNewBranch)
        {
            var currentbranch = new Command.Branch.GetCurrentBranch(this);
            var changes = new Command.WorkingTree.HasChanges(this);
            var rebasing = new Command.WorkingTree.IsRebaseActive(this);
            var merging = new Command.WorkingTree.IsMergeActive(this);
            currentbranch.WaitFor();
            changes.WaitFor();
            rebasing.WaitFor();
            merging.WaitFor();

            if (changes.stagedUncommittedFiles != false || changes.workingtreeChanges != false ||
                rebasing.result != false || merging.result != false ||
                currentbranch.DetachedHead != false)
            {
                return new CreateNewBranchResult()
                {
                    ErrorDetachedHead = (currentbranch.DetachedHead != false),
                    ErrorStagedUncommittedFiles = (changes.stagedUncommittedFiles != false),
                    ErrorWorkingTreeChanges = (changes.workingtreeChanges != false),
                    ErrorRebaseInProgress = (rebasing.result != false),
                    ErrorMergeInProgress = (merging.result != false),
                };
            }

            {
                var command = new Command.Branch.ListBranches(this, false, branchname);
                command.WaitFor();

                if (command.result.Count > 0)
                {
                    return new CreateNewBranchResult()
                    {
                        ErrorBranchAlreadyExists = true,
                    };
                }
            }

            var create = new Command.Branch.CreateBranch(this, branchname, branchFromBranchName, checkoutNewBranch);
            create.WaitFor();

            currentbranch = new Command.Branch.GetCurrentBranch(this);
            bool created = false;
            {
                var command = new Command.Branch.ListBranches(this, false, branchname);
                command.WaitFor();

                created = (command.result.Count > 0);
            }
            currentbranch.WaitFor();

            return new CreateNewBranchResult()
            {
                Created = created,
                CurrentBranch = currentbranch.shortname,

                ErrorCreating = !created,
            };
        }

        private bool CreateDeletedBranchUndoTag(string branchName, string mainBranch, string message)
        {
            string tagname;
            Command.Tag.ListTags list;
            GitTag found = null;
            do
            {
                tagname =
                    "gitnoob-deleted-branch-" + GitUtils.GenerateRandomSha1();
                list = new Command.Tag.ListTags(this);
                list.WaitFor();

                found = null;
                foreach (var tag in list.result)
                {
                    if (tag.Value.ShortName == tagname)
                    {
                        found = tag.Value;
                        break;
                    }
                }
            } while (found != null);

            var createtag = new Command.Tag.CreateTagToLastCommitOfBranch(this, branchName, tagname,
                "deleted-branch [" + GitUtils.EncodeUtf8Base64(branchName) + "]" +
                "[" + GitUtils.EncodeUtf8Base64(mainBranch) + "]" +
                "[" + DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'") + "]" +
                "[" + GitUtils.EncodeUtf8Base64(message) + "]"
                );
            createtag.WaitFor();

            list = new Command.Tag.ListTags(this);
            list.WaitFor();

            found = null;
            foreach (var tag in list.result)
            {
                if (tag.Value.ShortName == tagname)
                {
                    found = tag.Value;
                    break;
                }
            }

            if (found == null)
            {
                return false;
            }

            return true;
        }

        public CreateUndeletionTagResult CreateUndeletionTagOnCurrentBranch(string message)
        {
            var currentbranch = new Command.Branch.GetCurrentBranch(this);
            var changes = new Command.WorkingTree.HasChanges(this);
            var rebasing = new Command.WorkingTree.IsRebaseActive(this);
            var merging = new Command.WorkingTree.IsMergeActive(this);
            currentbranch.WaitFor();
            changes.WaitFor();
            rebasing.WaitFor();
            merging.WaitFor();

            if (changes.stagedUncommittedFiles != false || changes.workingtreeChanges != false ||
                rebasing.result != false || merging.result != false ||
                currentbranch.DetachedHead != false)
            {
                return new CreateUndeletionTagResult()
                {
                    ErrorDetachedHead = (currentbranch.DetachedHead != false),
                    ErrorStagedUncommittedFiles = (changes.stagedUncommittedFiles != false),
                    ErrorWorkingTreeChanges = (changes.workingtreeChanges != false),
                    ErrorRebaseInProgress = (rebasing.result != false),
                    ErrorMergeInProgress = (merging.result != false),
                };
            }

            if (!CreateDeletedBranchUndoTag(currentbranch.shortname, MainBranch, message))
            {
                return new CreateUndeletionTagResult()
                {
                    ErrorCreatingSafetyTag = true,
                };
            }

            return new CreateUndeletionTagResult()
            {
                Created = true,
            };
        }

        public DeleteCurrentBranchResult DeleteCurrentBranch(string branchname, string message)
        {
            var currentbranch = new Command.Branch.GetCurrentBranch(this);
            var changes = new Command.WorkingTree.HasChanges(this);
            var rebasing = new Command.WorkingTree.IsRebaseActive(this);
            var merging = new Command.WorkingTree.IsMergeActive(this);
            currentbranch.WaitFor();
            changes.WaitFor();
            rebasing.WaitFor();
            merging.WaitFor();

            if (changes.stagedUncommittedFiles != false || changes.workingtreeChanges != false ||
                rebasing.result != false || merging.result != false ||
                currentbranch.DetachedHead != false)
            {
                return new DeleteCurrentBranchResult()
                {
                    ErrorDetachedHead = (currentbranch.DetachedHead != false),
                    ErrorStagedUncommittedFiles = (changes.stagedUncommittedFiles != false),
                    ErrorWorkingTreeChanges = (changes.workingtreeChanges != false),
                    ErrorRebaseInProgress = (rebasing.result != false),
                    ErrorMergeInProgress = (merging.result != false),
                };
            }

            if (branchname != currentbranch.shortname)
            {
                return new DeleteCurrentBranchResult()
                {
                    ErrorCurrentBranchHasChanged = true,
                };
            }

            if (branchname == MainBranch)
            {
                return new DeleteCurrentBranchResult()
                {
                    ErrorCannotDeleteMainBranch = true,
                };
            }

            if (!CreateDeletedBranchUndoTag(branchname, MainBranch, message))
            {
                return new DeleteCurrentBranchResult()
                {
                    ErrorCreatingSafetyTag = true,
                };
            }

            var checkoutMainBranch = new Command.Branch.ChangeBranchTo(this, MainBranch);
            checkoutMainBranch.WaitFor();

            var delete = new Command.Branch.DeleteBranch(this, branchname, true);
            delete.WaitFor();

            var branchesCommand = new Command.Branch.ListBranches(this, false, branchname);
            branchesCommand.WaitFor();
            if (branchesCommand.result.Count != 0)
            {
                return new DeleteCurrentBranchResult()
                {
                    ErrorDeleting = true,
                };
            }

            return new DeleteCurrentBranchResult()
            {
                Deleted = true,
            };
        }

        public DeletedBranchesResult RetrieveDeletedBranches()
        {
            var result = new DeletedBranchesResult();

            var command = new Command.Tag.ListTags(this);
            command.WaitFor();
            if (command.result != null)
            {
                foreach (var item in command.result)
                {
                    var tag = item.Value;

                    if (tag.ShortName.StartsWith("gitnoob-deleted-branch-"))
                    {
                        result.DeletedBranches.Add(new GitDeletedBranch(tag));
                    }
                }
            }

            result.Sort();

            return result;
        }

        public CreateNewBranchResult UndeleteBranch(GitDeletedBranch deleted, string branchname, bool checkoutNewBranch)
        {
            var result = CreateNewBranch(branchname, deleted.Tag.PointingToCommitId, checkoutNewBranch);
            if (!result.Created) return result;

            var deltag = new Command.Tag.DeleteLocalTag(this, deleted.Tag.ShortName);
            deltag.WaitFor();

            return result;
        }

        public MoveUnpushedCommitsAndWorkingTreeChangesFromCurrentRemoteTrackingBranchToNewBranchResult MoveUnpushedCommitsAndWorkingTreeChangesFromCurrentRemoteTrackingBranchToNewBranch(string currentBranch, string newBranch)
        {
            var currentbranch = new Command.Branch.GetCurrentBranch(this);
            var remotebranch = new Command.Branch.GetRemoteBranch(this, currentBranch);
            currentbranch.WaitFor();
            remotebranch.WaitFor();
            if (currentbranch.DetachedHead != false ||
                currentbranch.shortname != currentBranch ||
                String.IsNullOrWhiteSpace(remotebranch.result)
                )
            {
                return new MoveUnpushedCommitsAndWorkingTreeChangesFromCurrentRemoteTrackingBranchToNewBranchResult()
                {
                    CurrentBranch = currentbranch.shortname,
                    ErrorDetachedHead = currentbranch.DetachedHead != false,
                    ErrorUnexpectedCurrentBranch = currentbranch.shortname != currentBranch,
                    ErrorNotTrackingRemoteBranch = String.IsNullOrWhiteSpace(remotebranch.result),
                };
            }

            var org_remote = remotebranch.result;

            var rename = new Command.Branch.RenameBranch(this, currentBranch, newBranch);
            rename.WaitFor();

            currentbranch = new Command.Branch.GetCurrentBranch(this);
            currentbranch.WaitFor();

            if (rename.result != true ||
                currentbranch.shortname != newBranch)
            {
                return new MoveUnpushedCommitsAndWorkingTreeChangesFromCurrentRemoteTrackingBranchToNewBranchResult()
                {
                    CurrentBranch = currentbranch.shortname,
                    ErrorRenaming = true,
                };
            }

            var removetracking = new Command.Branch.RemoveTrackingRemoteBranch(this, newBranch);
            removetracking.WaitFor();

            remotebranch = new Command.Branch.GetRemoteBranch(this, newBranch);
            remotebranch.WaitFor();

            if (!String.IsNullOrWhiteSpace(remotebranch.result))
            {
                return new MoveUnpushedCommitsAndWorkingTreeChangesFromCurrentRemoteTrackingBranchToNewBranchResult()
                {
                    CurrentBranch = currentbranch.shortname,
                    ErrorRemovingRemote = true,
                };
            }

            //recreate tracking branch (might be the main branch)
            var create = new Command.Branch.CreateBranch(this, currentBranch, org_remote, false);
            create.WaitFor();

            return new MoveUnpushedCommitsAndWorkingTreeChangesFromCurrentRemoteTrackingBranchToNewBranchResult()
            {
                CurrentBranch = currentbranch.shortname,
                Moved = true,
            };
        }
    }
}
