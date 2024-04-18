using GitNoob.GitResult;
using GitNoob.Utils;
using System;

namespace GitNoob.Git
{
    public partial class GitWorkingDirectory
    {
        public ChangeCurrentBranchResult ChangeCurrentBranchTo(string branchname)
        {
            //explicitly no check for detached head.
            //to allow for resolving detached head state by checking out a branch.
            var result = new ChangeCurrentBranchResult();
            if (GitDisaster.Check(this, result, new GitDisasterAllowed()
            {
                Allow_DetachedHead = true,
                Allow_UnpushedCommitsOnMainBranch = true,

                Allow_StagedUncommittedFiles = true,
                Allow_WorkingTreeChanges = true,
            }))
                return result;


            if (result.GitDisaster_StagedUncommittedFiles.Value || result.GitDisaster_WorkingTreeChanges.Value)
            {
                // Create temporary commit
                result.TemporaryCommitResult = CommitAllChangesOnCurrentBranch(null);
                if (!result.TemporaryCommitResult.Committed)
                {
                    result.TemporaryCommitFailed = true;
                    return result;
                }

                if (GitDisaster.Check(this, result, new GitDisasterAllowed()
                {
                    Allow_DetachedHead = true,
                    Allow_UnpushedCommitsOnMainBranch = true,
                }))
                    return result;
            }

            if (branchname.Contains("/"))
            {
                branchname = branchname.Substring(branchname.LastIndexOf("/") + 1);
            }

            var command = new Command.Branch.ChangeBranchTo(this, branchname);
            command.WaitFor();

            var currentbranch = new Command.Branch.GetCurrentBranch(this);
            currentbranch.WaitFor();
            result.CurrentBranch = currentbranch.shortname;

            if (currentbranch.shortname != branchname)
            {
                result.ErrorChanging = true;

                return result;
            }

            result.Changed = true;
            return result;
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
            var result = new EnsureMainBranchExistanceResult();
            if (GitDisaster.Check(this, result, GitDisasterAllowed.AllowAll()))
                return result;

            var list = new Command.Branch.ListBranches(this, true);
            list.WaitFor();

            GitBranch remote = null;
            foreach (var branch in list.result)
            {
                switch (branch.Type)
                {
                    case GitBranch.BranchType.Local:
                        if (string.IsNullOrEmpty(RemoteUrl))
                        {
                            result.Exists = true;
                            return result;
                        }
                        break;

                    case GitBranch.BranchType.LocalTrackingRemoteBranch:
                        if (branch.ShortName == MainBranch)
                        {
                            result.Exists = true;
                            return result;
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

            if (string.IsNullOrEmpty(RemoteUrl))
            {
                result.ErrorLocalBranchNotFound = true;

                return result;
            }

            if (remote == null)
            {
                result.ErrorRemoteBranchNotFound = true;

                return result;
            }

            var create = new Command.Branch.CreateBranch(this, MainBranch, remote.FullName, false);
            create.WaitFor();

            list = new Command.Branch.ListBranches(this, false, MainBranch);
            list.WaitFor();

            if (list.result.Count != 1)
            {
                result.ErrorCreatingMainBranch = true;

                return result;
            }

            if (list.result[0].Type != GitBranch.BranchType.LocalTrackingRemoteBranch)
            {
                result.ErrorCreatingMainBranch = true;

                return result;
            }

            result.Exists = true;
            return result;
        }

        public ResetMainBranchToRemoteResult ResetMainBranchToRemote()
        {
            var result = new ResetMainBranchToRemoteResult();
            if (GitDisaster.Check(this, result, new GitDisasterAllowed()
            {
                Allow_UnpushedCommitsOnMainBranch = true,
            }))
                return result;

            if (result.GitDisaster_CurrentBranchShortName == MainBranch)
            {
                result.ErrorCurrentBranchIsMainBranch = true;

                return result;
            }

            var currentCommit = new Command.Branch.GetLastCommitOfBranch(this, result.GitDisaster_CurrentBranchShortName);
            var mainCommit = new Command.Branch.GetLastCommitOfBranch(this, MainBranch);
            currentCommit.WaitFor();
            mainCommit.WaitFor();

            if (string.IsNullOrWhiteSpace(currentCommit.commitid) || string.IsNullOrWhiteSpace(mainCommit.commitid))
            {
                result.ErrorCurrentBranchCommitUnequalsMainBranchCommit = true;

                return result;
            }

            if (currentCommit.commitid != mainCommit.commitid)
            {
                result.ErrorCurrentBranchCommitUnequalsMainBranchCommit = true;

                return result;
            }

            {
                //forced delete local main branch
                var delete = new Command.Branch.DeleteBranch(this, MainBranch, true);
                delete.WaitFor();

                //keep current branch
                var checkout = new Command.Branch.ChangeBranchTo(this, result.GitDisaster_CurrentBranchShortName);
                checkout.WaitFor();
            }

            result.Reset = true;
            return result;
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
            var result = new SetRemoteForBranchResult();
            if (GitDisaster.Check(this, result, GitDisasterAllowed.AllowAll()))
                return result;

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
                        result.ErrorSettingRemoteUrl = true;

                        return result;
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
                    result.ErrorSettingRemoteForBranch = true;

                    return result;
                }
            }

            result.RemoteSet = true;
            return result;
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
            var result = new RenameBranchResult();
            if (GitDisaster.Check(this, result, new GitDisasterAllowed()
            {
                Allow_UnpushedCommitsOnMainBranch = true,
                Allow_StagedUncommittedFiles = true,
                Allow_WorkingTreeChanges = true,
            }))
                return result;

            var rename = new Command.Branch.RenameBranch(this, branchname, newname);
            rename.WaitFor();

            if (rename.result != true)
            {
                result.ErrorRenaming = true;

                return result;
            }

            result.Renamed = true;
            return result;
        }

        public CreateNewBranchResult CreateNewBranch(string branchname, string branchFromBranchName, bool checkoutNewBranch)
        {
            var result = new CreateNewBranchResult();
            if (checkoutNewBranch)
            {
                if (GitDisaster.Check(this, result, new GitDisasterAllowed()
                {
                    Allow_DetachedHead = true,
                    Allow_UnpushedCommitsOnMainBranch = true,

                    Allow_StagedUncommittedFiles = true,
                    Allow_WorkingTreeChanges = true,
                }))
                    return result;


                if (result.GitDisaster_StagedUncommittedFiles.Value || result.GitDisaster_WorkingTreeChanges.Value)
                {
                    // Create temporary commit
                    result.TemporaryCommitResult = CommitAllChangesOnCurrentBranch(null);
                    if (!result.TemporaryCommitResult.Committed)
                    {
                        result.TemporaryCommitFailed = true;
                        return result;
                    }

                    if (GitDisaster.Check(this, result, new GitDisasterAllowed()
                    {
                        Allow_DetachedHead = true,
                        Allow_UnpushedCommitsOnMainBranch = true,
                    }))
                        return result;
                }
            }

            {
                var command = new Command.Branch.ListBranches(this, false, branchname);
                command.WaitFor();

                if (command.result.Count > 0)
                {
                    result.ErrorBranchAlreadyExists = true;

                    return result;
                }
            }

            var create = new Command.Branch.CreateBranch(this, branchname, branchFromBranchName, checkoutNewBranch);
            create.WaitFor();

            var currentbranch = new Command.Branch.GetCurrentBranch(this);
            bool created;
            {
                var command = new Command.Branch.ListBranches(this, false, branchname);
                command.WaitFor();

                created = (command.result.Count > 0);
            }
            currentbranch.WaitFor();

            result.Created = created;
            result.CurrentBranch = currentbranch.shortname;
            result.ErrorCreating = !created;
            return result;
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
            var result = new CreateUndeletionTagResult();
            if (GitDisaster.Check(this, result, new GitDisasterAllowed()))
                return result;

            if (!CreateDeletedBranchUndoTag(result.GitDisaster_CurrentBranchShortName, MainBranch, message))
            {
                result.ErrorCreatingSafetyTag = true;

                return result;
            }

            result.Created = true;
            return result;
        }

        public DeleteBranchResult DeleteBranch(string branchname, string message)
        {
            var result = new DeleteBranchResult();
            if (GitDisaster.Check(this, result, new GitDisasterAllowed()))
                return result;

            if (branchname == MainBranch)
            {
                result.ErrorCannotDeleteMainBranch = true;
                return result;
            }

            if (!CreateDeletedBranchUndoTag(branchname, MainBranch, message))
            {
                result.ErrorCreatingSafetyTag = true;
                return result;
            }

            if (branchname == result.GitDisaster_CurrentBranchShortName)
            {
                var checkoutMainBranch = new Command.Branch.ChangeBranchTo(this, MainBranch);
                checkoutMainBranch.WaitFor();

                var branch = new Command.Branch.GetCurrentBranch(this);
                branch.WaitFor();

                if (branch.shortname != MainBranch)
                {
                    result.ErrorChangingToMainBranch = true;

                    return result;
                }
            }

            var delete = new Command.Branch.DeleteBranch(this, branchname, true);
            delete.WaitFor();

            var branchesCommand = new Command.Branch.ListBranches(this, false, branchname);
            branchesCommand.WaitFor();
            if (branchesCommand.result.Count != 0)
            {
                result.ErrorDeleting = true;
                return result;
            }

            result.Deleted = true;
            return result;
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

        public MoveUnpushedCommitsFromRemoteTrackingBranchToNewBranchResult MoveUnpushedCommitsFromRemoteTrackingBranchToNewBranch(string remoteTrackingBranch, string newBranch)
        {
            var result = new MoveUnpushedCommitsFromRemoteTrackingBranchToNewBranchResult();
            if (GitDisaster.Check(this, result, new GitDisasterAllowed()
            {
                Allow_UnpushedCommitsOnMainBranch = true,
                Allow_WorkingTreeChanges = true,
                Allow_StagedUncommittedFiles = true,
            }))
                return result;

            var remotebranch = new Command.Branch.GetRemoteBranch(this, remoteTrackingBranch);
            remotebranch.WaitFor();

            if (String.IsNullOrWhiteSpace(remotebranch.result))
            {
                result.ErrorNotTrackingRemoteBranch = true;

                return result;
            }

            bool isCurrentBranch = result.GitDisaster_CurrentBranchShortName == remoteTrackingBranch;
            var org_remote = remotebranch.result;

            var rename = new Command.Branch.RenameBranch(this, remoteTrackingBranch, newBranch);
            rename.WaitFor();

            if (rename.result != true)
            {
                result.ErrorRenaming = true;

                return result;
            }

            if (isCurrentBranch)
            {
                var currentbranch = new Command.Branch.GetCurrentBranch(this);
                currentbranch.WaitFor();

                if (rename.result != true ||
                    currentbranch.shortname != newBranch)
                {
                    result.ErrorRenaming = true;
                    return result;
                }

                result.GitDisaster_CurrentBranchShortName = newBranch;
                result.GitDisaster_CurrentGitBranch = currentbranch.branch;
            }

            var removetracking = new Command.Branch.RemoveTrackingRemoteBranch(this, newBranch);
            removetracking.WaitFor();

            remotebranch = new Command.Branch.GetRemoteBranch(this, newBranch);
            remotebranch.WaitFor();

            if (!String.IsNullOrWhiteSpace(remotebranch.result))
            {
                result.ErrorRemovingRemote = true;

                return result;
            }

            //recreate tracking branch (might be the main branch)
            var create = new Command.Branch.CreateBranch(this, remoteTrackingBranch, org_remote, false);
            create.WaitFor();

            result.Moved = true;
            return result;
        }

    }
}
