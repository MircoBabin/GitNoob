using GitNoob.Git.Result;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace GitNoob.Git
{
    public class GitWorkingDirectory
    {
        private Config.WorkingDirectory _workingdirectory;

        public static string getGitExecutable()
        {
            return _gitExecutable;
        }

        public static void setGitExecutable(string fullPathTo_Git_Exe)
        {
            string installmsg = "Install Git for Windows from https://git-scm.com/. And publish in global \"path\"." + Environment.NewLine + Environment.NewLine;

            if (fullPathTo_Git_Exe == null) throw new Exception(installmsg + "Executable git.exe not found in \"path\".");

            _gitExecutable = fullPathTo_Git_Exe;
            if (!File.Exists(_gitExecutable)) throw new Exception(installmsg + "Executable \"" + fullPathTo_Git_Exe + "\" does not exist.");

            var cmd = new Command.GetVersionOfGit(_gitExecutable);
            cmd.WaitFor();

            var expectMajor = 2;
            var expectMinor = 34;
            var expectPatch = 1;
            string versionmsg = installmsg +
                                "Installed Git version should be at least " + expectMajor + "." + expectMinor + "." + expectPatch + Environment.NewLine +
                                Environment.NewLine +
                                "Current installed version is \"" + (cmd.fullversion != null ? cmd.fullversion : "") + "\"." + Environment.NewLine +
                                "(Current version major=" + (cmd.major != null ? cmd.major.ToString() : "") +
                                " ,minor=" + (cmd.minor != null ? cmd.minor.ToString() : "") +
                                " ,patch=" + (cmd.patch != null ? cmd.patch.ToString() : "") +
                                ").";
            if (cmd.major < expectMajor) throw new Exception(versionmsg);
            if (cmd.major == expectMajor)
            {
                if (cmd.minor < expectMinor) throw new Exception(versionmsg);
                if (cmd.minor == expectMinor)
                {
                    if (cmd.patch < expectPatch) throw new Exception(versionmsg);
                }
            }
        }

        private static string _gitExecutable = null;
        public string GitExecutable
        {
            get
            {
                return _gitExecutable;
            }
        }

        public Config.WorkingDirectory ConfigWorkingDirectory
        {
            get
            {
                return _workingdirectory;
            }
        }

        public string WorkingPath
        {
            get
            {
                return _workingdirectory.Path;
            }
        }

        public string RemoteUrl
        {
            get
            {
                return _workingdirectory.Git.RemoteUrl;
            }
        }

        public string MainBranch
        {
            get
            {
                return _workingdirectory.Git.MainBranch;
            }
        }


        public GitWorkingDirectory(Config.WorkingDirectory workingdirectory)
        {
            if (_gitExecutable == null)
            {
                throw new Exception("git.exe path not set - use setGitExecutable()");
            }

            _workingdirectory = workingdirectory;
        }

        public StatusResult RetrieveStatus()
        {
            if (!Directory.Exists(_workingdirectory.Path))
            {
                return new StatusResult();
            }

            var rootdir = new Command.WorkingTree.IsGitRootDirectory(this);
            rootdir.WaitFor();
            if (rootdir.result != true)
            {
                return new StatusResult
                {
                    DirectoryExists = true,
                };
            }

            var currentbranch = new Command.Branch.GetCurrentBranch(this);
            var commitname = new Command.Config.GetCurrentCommitter(this);
            var changes = new Command.WorkingTree.HasChanges(this);
            var rebasing = new Command.WorkingTree.IsRebaseActive(this);
            var merging = new Command.WorkingTree.IsMergeActive(this);
            var conflicts = new Command.WorkingTree.HasConflicts(this);
            var mainbranchcommit = new Command.Branch.GetLastCommitOfBranch(this, MainBranch);
            var mainbranchremote = new Command.Branch.GetRemoteBranch(this, MainBranch);

            currentbranch.WaitFor();
            commitname.WaitFor();
            changes.WaitFor();
            rebasing.WaitFor();
            merging.WaitFor();
            conflicts.WaitFor();
            mainbranchcommit.WaitFor();
            mainbranchremote.WaitFor();

            return new StatusResult
            {
                DirectoryExists = true,
                IsGitRootDirectory = true,
                DetachedHead_NotOnBranch = (currentbranch.DetachedHead == true),
                CurrentBranch = currentbranch.shortname,
                CommitFullName = commitname.result,
                CommitName = commitname.name,
                CommitEmail = commitname.email,
                HasWorkingTreeChanges = (changes.workingtreeChanges == true), //modified, deleted and/or untracked files
                HasStagedUncommittedFiles = (changes.stagedUncommittedFiles == true),
                Rebasing = (rebasing.result == true),
                Merging = (merging.result == true),
                Conflicts = (conflicts.result == true),
                MainBranchExists = (!string.IsNullOrEmpty(mainbranchcommit.commitid)),
                MainBranchIsTrackingRemoteBranch = (!string.IsNullOrEmpty(mainbranchremote.result)),
            };
        }

        public ChangeCommitterResult ChangeCommitter(string toName, string toEmail)
        {
            var set = new Command.Config.SetCommitter(this, toName, toEmail);
            set.WaitFor();

            var commitname = new Command.Config.GetCurrentCommitter(this);
            commitname.WaitFor();

            return new ChangeCommitterResult
            {
                Changed = (commitname.name == toName && commitname.email == toEmail),

                CommitFullName = commitname.result,
                CommitName = commitname.name,
                CommitEmail = commitname.email,

                ErrorChangingName = (commitname.name != toName),
                ErrorChangingEmail = (commitname.email != toEmail),
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

            var currentbranch = new Command.Branch.GetCurrentBranch(this);
            var command = new Command.Branch.ListBranches(this, true);
            command.WaitFor();
            currentbranch.WaitFor();

            result.CurrentBranch = currentbranch.shortname;

            if (command.result != null)
            {
                foreach(var branch in command.result)
                {
                    result.Branches.Add(branch.ShortName);
                }
            }

            return result;
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

            var create = new Command.Branch.CreateBranch(this, branchname, branchFromBranchName, checkoutNewBranch);
            create.WaitFor();

            currentbranch = new Command.Branch.GetCurrentBranch(this);
            var command = new Command.Branch.ListBranches(this, true);
            command.WaitFor();
            currentbranch.WaitFor();

            bool created = false;
            if (command.result != null)
            {
                foreach (var branch in command.result)
                {
                    if (branch.ShortName == branchname)
                    {
                        created = true;
                        break;
                    }
                }
            }

            return new CreateNewBranchResult()
            {
                Created = created,
                CurrentBranch = currentbranch.shortname,

                ErrorCreating = !created,
            };
        }

        public ChangeCurrentBranchResult ChangeCurrentBranchTo(string branchname)
        {
            //explicitly no check for detached head.
            //to allow for resolving detached head state by checking out a branch.
            var changes = new Command.WorkingTree.HasChanges(this);
            var rebasing = new Command.WorkingTree.IsRebaseActive(this);
            var merging = new Command.WorkingTree.IsMergeActive(this);

            changes.WaitFor();
            rebasing.WaitFor();
            merging.WaitFor();

            if (changes.stagedUncommittedFiles != false || changes.workingtreeChanges != false || 
                rebasing.result != false || merging.result != false)
            {
                return new ChangeCurrentBranchResult()
                {
                    ErrorStagedUncommittedFiles = (changes.stagedUncommittedFiles != false),
                    ErrorWorkingTreeChanges = (changes.workingtreeChanges != false),
                    ErrorRebaseInProgress = (rebasing.result != false),
                    ErrorMergeInProgress = (merging.result != false),
                };
            }

            if (branchname.Contains("/"))
            {
                branchname = branchname.Substring(branchname.LastIndexOf("/") + 1);
            }

            var command = new Command.Branch.ChangeBranchTo(this, branchname);
            command.WaitFor();

            var currentbranch = new Command.Branch.GetCurrentBranch(this);
            currentbranch.WaitFor();

            if (currentbranch.shortname != branchname)
            {
                return new ChangeCurrentBranchResult()
                {
                    ErrorChanging = true,
                };
            }

            return new ChangeCurrentBranchResult()
            {
                Changed = true,
                CurrentBranch = currentbranch.shortname,
            };
        }

        public UnpackLastTemporaryCommitOnCurrentBranchResult UnpackLastTemporaryCommitOnCurrentBranch()
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
                return new UnpackLastTemporaryCommitOnCurrentBranchResult()
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
            if (lastcommit.commitmessage == null || !lastcommit.commitmessage.StartsWith(TemporaryCommitMessage))
            {
                return new UnpackLastTemporaryCommitOnCurrentBranchResult()
                {
                    CurrentBranch = currentbranch.shortname,

                    NoTemporaryCommitToUnpack = true,
                };
            }

            var unpack = new Command.Branch.ResetCurrentBranchToPreviousCommit(this);
            unpack.WaitFor();

            return new UnpackLastTemporaryCommitOnCurrentBranchResult()
            {
                CurrentBranch = currentbranch.shortname,

                Unpacked = true,
            };
        }

        private GetLatestResult GetLatestViaClone()
        {
            var clonecmd = new Command.Repository.Clone(this);
            clonecmd.WaitFor();

            ClearCache();

            var rootdir = new Command.WorkingTree.IsGitRootDirectory(this, false);
            rootdir.WaitFor();
            if (rootdir.result != true)
            {
                try
                {
                    Directory.Delete(_workingdirectory.Path, true);
                }
                catch { }

                throw new Exception("Error cloning into directory: " + _workingdirectory.Path);
            }

            var commitname = new Command.Config.GetCurrentCommitter(this);
            commitname.WaitFor();

            return new GetLatestResult
            {
                Cloned = true,
                CommitFullName = commitname.result,
                CommitName = commitname.name,
                CommitEmail = commitname.email,
            };
        }

        private GetLatestResult GetLatestViaUpdate()
        {
            var rootdir = new Command.WorkingTree.IsGitRootDirectory(this, false);
            rootdir.WaitFor();
            if (rootdir.result != true)
            {
                return new GetLatestResult
                {
                    ErrorNonEmptyAndNotAGitRepository = true,
                };
            }

            var changes = new Command.WorkingTree.HasChanges(this);
            var unpushedCommitsOnMain = new Command.Branch.HasBranchUnpushedChanges(this, MainBranch, null);

            var currentbranch = new Command.Branch.GetCurrentBranch(this);
            currentbranch.WaitFor();

            Command.Branch.HasBranchUnpushedChanges unpushedCommits;
            if (currentbranch.shortname != MainBranch)
            {
                unpushedCommits = new Command.Branch.HasBranchUnpushedChanges(this, currentbranch.shortname, MainBranch);
                unpushedCommits.WaitFor();
            }
            else
            {
                unpushedCommits = unpushedCommitsOnMain;
            }

            changes.WaitFor();
            unpushedCommitsOnMain.WaitFor();

            bool isRemoteTrackingBranch = !String.IsNullOrWhiteSpace(unpushedCommits.RemoteBranch.result);
            bool stagedResult = (changes.stagedUncommittedFiles != false);
            bool unpushedOnMainResult = (unpushedCommitsOnMain.result == true) || (unpushedCommitsOnMain.IsTrackingRemoteBranch == Command.Branch.HasBranchUnpushedChanges.TrackingRemoteBranch.NoAndHasLocalCommits);
            bool unpushedResult = (isRemoteTrackingBranch && unpushedCommits.result == true);
            bool workingtreeResult = ((isRemoteTrackingBranch || currentbranch.shortname == MainBranch) && changes.workingtreeChanges != false);
            if (stagedResult ||
                unpushedOnMainResult ||
                unpushedResult ||
                workingtreeResult)
            {
                return new GetLatestResult
                {
                    CurrentBranch = currentbranch.shortname,
                    WorkingTreeChanges = (changes.workingtreeChanges != false),
                    StagedUncommittedFiles = (changes.stagedUncommittedFiles != false),
                    ErrorStagedUncommittedFiles = stagedResult,
                    ErrorUnpushedCommitsOnMainBranch = unpushedOnMainResult,
                    ErrorUnpushedCommitsAndOnLocalTrackingRemoteBranch = unpushedResult,
                    ErrorWorkingTreeChangesAndOnLocalTrackingRemoteBranch = workingtreeResult
                };
            }

            var fetch = new Command.Repository.UpdateRepositoryFromRemotes(this);
            fetch.WaitFor();

            bool CurrentBranchIsBehindMainBranch;
            var commitname = new Command.Config.GetCurrentCommitter(this);
            if (MainBranch == currentbranch.shortname)
            {
                var fastForwardMainBranch = new Command.Branch.FastForwardCurrentBranchToRemote(this);
                fastForwardMainBranch.WaitFor();

                CurrentBranchIsBehindMainBranch = false;
            }
            else
            {
                var common = new Command.Branch.FindCommonCommitOfTwoBranches(this, MainBranch, currentbranch.shortname);
                var maincommit = new Command.Branch.GetLastCommitOfBranch(this, MainBranch);
                common.WaitFor();
                maincommit.WaitFor();

                CurrentBranchIsBehindMainBranch = (common.commitid != maincommit.commitid);
            }
            commitname.WaitFor();

            return new GetLatestResult()
            {
                Updated = true,
                CurrentBranch = currentbranch.shortname,
                WorkingTreeChanges = (changes.workingtreeChanges != false),
                StagedUncommittedFiles = (changes.stagedUncommittedFiles != false),
                UnpushedCommits = (unpushedCommits.result == true),
                CurrentBranchIsBehindMainBranch = CurrentBranchIsBehindMainBranch,
                CommitFullName = commitname.result,
                CommitName = commitname.name,
                CommitEmail = commitname.email,
            };
        }

        private bool IsDirectoryEmpty(string path)
        {
            foreach (var item in Directory.EnumerateFileSystemEntries(path))
            {
                return false;
            }

            return true;
        }


        public GetLatestResult GetLatest(bool CheckGitCredentialsViaKeePassCommander = true)
        {
            bool clone;
            if (!Directory.Exists(_workingdirectory.Path))
            {
                try
                {
                    Directory.CreateDirectory(_workingdirectory.Path);
                    if (!Directory.Exists(_workingdirectory.Path))
                    {
                        throw new Exception("Error creating directory: " + _workingdirectory.Path);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error creating directory: " + _workingdirectory.Path, ex);
                }

                clone = true;
            }
            else
            {
                clone = IsDirectoryEmpty(_workingdirectory.Path);
            }

            if (CheckGitCredentialsViaKeePassCommander && !GitCredentialsViaKeePassCommander.AreCredentialsAvailable(this))
            {
                return new GetLatestResult
                {
                    ErrorKeePassNotStarted = true,
                };
            }

            var reachable = new Command.Repository.RemoteReachable(this);
            reachable.WaitFor();
            if (reachable.result != true)
            {
                return new GetLatestResult
                {
                    ErrorRemoteNotReachable = true,
                };
            }

            if (clone)
            {
                return GetLatestViaClone();
            }
            else
            {
                return GetLatestViaUpdate();
            }
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

        public RebaseResult RebaseCurrentBranchOntoMainBranch()
        {
            var currentbranch = new Command.Branch.GetCurrentBranch(this);
            var changes = new Command.WorkingTree.HasChanges(this);
            var unpushedCommitsOnMain = new Command.Branch.HasBranchUnpushedChanges(this, MainBranch, null);
            var rebasing = new Command.WorkingTree.IsRebaseActive(this);
            var merging = new Command.WorkingTree.IsMergeActive(this);
            currentbranch.WaitFor();
            changes.WaitFor();
            unpushedCommitsOnMain.WaitFor();
            rebasing.WaitFor();
            merging.WaitFor();

            if (changes.stagedUncommittedFiles != false || changes.workingtreeChanges != false || unpushedCommitsOnMain.result != false || rebasing.result != false || merging.result != false || currentbranch.DetachedHead != false)
            {
                return new RebaseResult()
                {
                    CurrentBranch = currentbranch.shortname,

                    ErrorDetachedHead = (currentbranch.DetachedHead != false),
                    ErrorStagedUncommittedFiles = (changes.stagedUncommittedFiles != false),
                    ErrorWorkingTreeChanges = (changes.workingtreeChanges != false),
                    ErrorUnpushedCommitsOnMainBranch = (unpushedCommitsOnMain.result != false),
                    ErrorRebaseInProgress = (rebasing.result != false),
                    ErrorMergeInProgress = (merging.result != false),
                };
            }

            var rebase = new Command.Branch.RebaseCurrentBranch(this, MainBranch);
            rebase.WaitFor();

            var conflicts = new Command.WorkingTree.HasConflicts(this);
            conflicts.WaitFor();
            if (conflicts.result != false)
            {
                return new RebaseResult()
                {
                    CurrentBranch = currentbranch.shortname,

                    ErrorConflicts = true,
                };
            }

            return new RebaseResult()
            {
                CurrentBranch = currentbranch.shortname,
                Rebased = true,
            };
        }

        public RebaseResult RebaseContinue()
        {
            var rebasing = new Command.WorkingTree.IsRebaseActive(this);
            rebasing.WaitFor();

            if (rebasing.result != true)
            {
                return new RebaseResult()
                {
                    ErrorNotRebasing = true,
                };
            }

            var rebase = new Command.Branch.RebaseContinue(this);
            rebase.WaitFor();

            var conflicts = new Command.WorkingTree.HasConflicts(this);
            conflicts.WaitFor();
            if (conflicts.result != false)
            {
                return new RebaseResult()
                {
                    ErrorConflicts = true,
                };
            }

            return new RebaseResult()
            {
                Rebased = true,
            };
        }

        public RebaseResult RebaseAbort()
        {
            var rebasing = new Command.WorkingTree.IsRebaseActive(this);
            rebasing.WaitFor();

            if (rebasing.result != true)
            {
                return new RebaseResult()
                {
                    ErrorNotRebasing = true,
                };
            }

            var rebase = new Command.Branch.RebaseAbort(this);
            rebase.WaitFor();

            rebasing = new Command.WorkingTree.IsRebaseActive(this);
            rebasing.WaitFor();
            if (rebasing.result != false)
            {
                return new RebaseResult()
                {
                    Aborted = false,
                    ErrorRebaseInProgress = true,
                };
            }

            return new RebaseResult()
            {
                Aborted = true,
            };
        }

        public MergeResult MergeFastForwardOnlyCurrentBranchIntoMainBranch()
        {
            var currentbranch = new Command.Branch.GetCurrentBranch(this);
            var changes = new Command.WorkingTree.HasChanges(this);
            var unpushedCommitsOnMain = new Command.Branch.HasBranchUnpushedChanges(this, MainBranch, null);
            var rebasing = new Command.WorkingTree.IsRebaseActive(this);
            var merging = new Command.WorkingTree.IsMergeActive(this);
            currentbranch.WaitFor();
            changes.WaitFor();
            unpushedCommitsOnMain.WaitFor();
            rebasing.WaitFor();
            merging.WaitFor();

            if (changes.stagedUncommittedFiles != false || changes.workingtreeChanges != false || unpushedCommitsOnMain.result != false || rebasing.result != false || merging.result != false || currentbranch.DetachedHead != false)
            {
                return new MergeResult()
                {
                    CurrentBranch = currentbranch.shortname,

                    ErrorDetachedHead = (currentbranch.DetachedHead != false),
                    ErrorStagedUncommittedFiles = (changes.stagedUncommittedFiles != false),
                    ErrorWorkingTreeChanges = (changes.workingtreeChanges != false),
                    ErrorUnpushedCommitsOnMainBranch = (unpushedCommitsOnMain.result != false),
                    ErrorRebaseInProgress = (rebasing.result != false),
                    ErrorMergeInProgress = (merging.result != false),
                };
            }

            var lastcommit = new Command.Branch.GetLastCommitOfBranch(this, currentbranch.shortname);
            lastcommit.WaitFor();
            if (lastcommit.commitmessage == null)
            {
                return new MergeResult()
                {
                    CurrentBranch = currentbranch.shortname,

                    ErrorRetrievingLastCommit = true,
                };

            }
            bool lastCommitIsTemporary = lastcommit.commitmessage.StartsWith(TemporaryCommitMessage);

            Command.Branch.MergeFastForwardOnly merge;
            if (!lastCommitIsTemporary)
            {
                merge = new Command.Branch.MergeFastForwardOnly(this, currentbranch.shortname, MainBranch);
                merge.WaitFor();
                //Because of fast-forward-only there can be no conflicts.
            }
            else
            {
                //create branch on previous commit (last commit is a temporary commit)
                string randomsha1 = GitUtils.GenerateRandomSha1();
                string tempbranch = "gitnoob-temp-" + randomsha1;

                var newbranch = new Command.Branch.CreateBranch(this, tempbranch, currentbranch.shortname + "^", false);
                newbranch.WaitFor();
                try
                {
                    merge = new Command.Branch.MergeFastForwardOnly(this, tempbranch, MainBranch);
                    merge.WaitFor();
                    //Because of fast-forward-only there can be no conflicts.
                }
                finally
                {
                    var delnewbranch = new Command.Branch.DeleteBranch(this, tempbranch, true);
                    delnewbranch.WaitFor();
                }
            }

            //restore current branch
            var change = new Command.Branch.ChangeBranchTo(this, currentbranch.shortname);
            change.WaitFor();

            return new MergeResult()
            {
                CurrentBranch = currentbranch.shortname,
                Merged = merge.result == true,
            };
        }

        public MergeResult MergeContinue()
        {
            var merging = new Command.WorkingTree.IsMergeActive(this);
            merging.WaitFor();

            if (merging.result != true)
            {
                return new MergeResult()
                {
                    ErrorNotMerging = true,
                };
            }

            var merge = new Command.Branch.MergeContinue(this);
            merge.WaitFor();

            var conflicts = new Command.WorkingTree.HasConflicts(this);
            conflicts.WaitFor();
            if (conflicts.result != false)
            {
                return new MergeResult()
                {
                    ErrorConflicts = true,
                };
            }

            return new MergeResult()
            {
                Merged = true,
            };
        }

        public MergeResult MergeAbort(string ChangeBranchTo)
        {
            var merging = new Command.WorkingTree.IsMergeActive(this);
            merging.WaitFor();

            if (merging.result != true)
            {
                return new MergeResult()
                {
                    ErrorNotMerging = true,
                };
            }

            var merge = new Command.Branch.MergeAbort(this);
            merge.WaitFor();

            merging = new Command.WorkingTree.IsMergeActive(this);
            merging.WaitFor();
            if (merging.result != false)
            {
                return new MergeResult()
                {
                    Aborted = false,
                    ErrorMergeInProgress = true,
                };
            }

            if (!String.IsNullOrWhiteSpace(ChangeBranchTo))
            {
                var change = new Command.Branch.ChangeBranchTo(this, ChangeBranchTo);
                change.WaitFor();
            }

            return new MergeResult()
            {
                Aborted = true,
            };
        }

        public GitLockResult AcquireGitLockForMainBranch(string username, string message=null)
        {
            GitLock gitlock = null;
            try
            {
                gitlock = new GitLock(this, MainBranch);
            }
            catch
            {
                return new GitLockResult()
                {
                    ErrorRetrievingRemoteBranchName = true,
                };
            }

            return gitlock.Acquire(username, message);
        }

        public GitLockResult ResetGitLockForMainBranch()
        {
            GitLock gitlock = null;
            try
            {
                gitlock = new GitLock(this, MainBranch);
            }
            catch
            {
                return new GitLockResult()
                {
                    ErrorRetrievingRemoteBranchName = true,
                };
            }

            gitlock.Reset();

            return new GitLockResult()
            {
                Unlocked = true,
            };
        }

        public PushResult PushMainBranchToRemote(bool CheckGitCredentialsViaKeePassCommander = true)
        {
            var changes = new Command.WorkingTree.HasChanges(this);
            var rebasing = new Command.WorkingTree.IsRebaseActive(this);
            var merging = new Command.WorkingTree.IsMergeActive(this);
            changes.WaitFor();
            rebasing.WaitFor();
            merging.WaitFor();

            if (changes.stagedUncommittedFiles != false || changes.workingtreeChanges != false || rebasing.result != false || merging.result != false)
            {
                return new PushResult()
                {
                    ErrorStagedUncommittedFiles = (changes.stagedUncommittedFiles != false),
                    ErrorWorkingTreeChanges = (changes.workingtreeChanges != false),
                    ErrorRebaseInProgress = (rebasing.result != false),
                    ErrorMergeInProgress = (merging.result != false),
                };
            }

            if (CheckGitCredentialsViaKeePassCommander && !GitCredentialsViaKeePassCommander.AreCredentialsAvailable(this))
            {
                return new PushResult
                {
                    ErrorKeePassNotStarted = true,
                };
            }

            var reachable = new Command.Repository.RemoteReachable(this);
            reachable.WaitFor();
            if (reachable.result != true)
            {
                return new PushResult
                {
                    ErrorRemoteNotReachable = true,
                };
            }

            var push = new Command.Branch.PushBranchToRemote(this, MainBranch);
            push.WaitFor();
            if (push.output != null)
            {
                //fuzzy search for "non-fast-forward updates were rejected"
                string pattern = @"\b(?=non).(?=fast).(?=forward).*\b(?=rejected)";
                var matches = Regex.Matches(push.output, pattern, RegexOptions.IgnoreCase);
                if (matches.Count > 0)
                {
                    return new PushResult()
                    {
                        PushOutput = push.output,

                        ErrorConflicts = true,
                    };
                }
            }

            var unpushedCommitsOnMain = new Command.Branch.HasBranchUnpushedChanges(this, MainBranch, null);
            unpushedCommitsOnMain.WaitFor();

            if (unpushedCommitsOnMain.result != false)
            {
                return new PushResult()
                {
                    ErrorStillUnpushedCommitsOnMainBranch = true,
                };
            }

            return new PushResult()
            {
                Pushed = true,
            };
        }

        public EnsureMainBranchExistanceResult EnsureMainBranchExistance()
        {
            //checkout a (non existing) main branch will create it automatically tracking the remote branch

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
                return new EnsureMainBranchExistanceResult()
                {
                    ErrorDetachedHead = (currentbranch.DetachedHead != false),
                    ErrorStagedUncommittedFiles = (changes.stagedUncommittedFiles != false),
                    ErrorWorkingTreeChanges = (changes.workingtreeChanges != false),
                    ErrorRebaseInProgress = (rebasing.result != false),
                    ErrorMergeInProgress = (merging.result != false),
                };
            }

            var checkout = new Command.Branch.ChangeBranchTo(this, MainBranch);
            checkout.WaitFor();

            var branch = new Command.Branch.GetCurrentBranch(this);
            var remotebranch = new Command.Branch.GetRemoteBranch(this, MainBranch);
            branch.WaitFor();
            remotebranch.WaitFor();

            //restore current branch
            checkout = new Command.Branch.ChangeBranchTo(this, currentbranch.shortname);
            checkout.WaitFor();

            if (branch.shortname != MainBranch)
            {
                return new EnsureMainBranchExistanceResult()
                {
                    ErrorNotAutomaticallyCreated = true,
                };
            }

            if (String.IsNullOrEmpty(remotebranch.result))
            {
                return new EnsureMainBranchExistanceResult()
                {
                    ErrorNotTrackingRemoteBranch = true,
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

        public const string TemporaryCommitMessage = "[GitNoob][Temporary Commit]";

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


        public DeleteWorkingTreeChangesAndStagedUncommittedFilesResult DeleteWorkingTreeChangesAndStagedUncommittedFiles()
        {
            var rebasing = new Command.WorkingTree.IsRebaseActive(this);
            var merging = new Command.WorkingTree.IsMergeActive(this);
            rebasing.WaitFor();
            merging.WaitFor();

            if (rebasing.result != false ||
                merging.result != false)
            {
                return new DeleteWorkingTreeChangesAndStagedUncommittedFilesResult()
                {
                    ErrorRebaseInProgress = (rebasing.result != false),
                    ErrorMergeInProgress = (merging.result != false),
                };
            }

            {
                var changes = new Command.WorkingTree.DeleteAllWorkingTreeChangesAndStagedUncommittedFiles(this);
                changes.WaitFor();
            }

            {
                var changes = new Command.WorkingTree.HasChanges(this);
                changes.WaitFor();

                if (changes.stagedUncommittedFiles != false ||
                    changes.workingtreeChanges != false)
                {
                    return new DeleteWorkingTreeChangesAndStagedUncommittedFilesResult()
                    {
                        ErrorStillStagedUncommittedFiles = (changes.stagedUncommittedFiles != false),
                        ErrorStillWorkingTreeChanges = (changes.workingtreeChanges != false),
                    };
                }
            }

            return new DeleteWorkingTreeChangesAndStagedUncommittedFilesResult()
            {
                ChangesDeleted = true,
            };
        }

        public BuildCacheAndCommitOnMainBranchResult BuildCacheAndCommitOnMainBranch(Config.IExecutor Executor, string CommitMessage)
        {
            Config.IProjectType ProjectType = _workingdirectory.ProjectType;
            if (ProjectType == null || 
                !ProjectType.Capabilities.CapableOfClearAndBuildCache)
            {
                return new BuildCacheAndCommitOnMainBranchResult
                {
                    NotUpdatedBecauseNothingChanged = true,
                };
            }

            var currentbranch = new Command.Branch.GetCurrentBranch(this);
            var unpushedCommitsOnMain = new Command.Branch.HasBranchUnpushedChanges(this, MainBranch, null);
            var mainCommit = new Command.Branch.GetLastCommitOfBranch(this, MainBranch);
            var changes = new Command.WorkingTree.HasChanges(this);
            var rebasing = new Command.WorkingTree.IsRebaseActive(this);
            var merging = new Command.WorkingTree.IsMergeActive(this);
            currentbranch.WaitFor();
            changes.WaitFor();
            rebasing.WaitFor();
            merging.WaitFor();
            mainCommit.WaitFor();
            unpushedCommitsOnMain.WaitFor();

            if (changes.stagedUncommittedFiles != false || 
                changes.workingtreeChanges != false || 
                unpushedCommitsOnMain.result != false || 
                rebasing.result != false || 
                merging.result != false ||
                currentbranch.DetachedHead != false)
            {
                return new BuildCacheAndCommitOnMainBranchResult()
                {
                    ErrorDetachedHead = (currentbranch.DetachedHead != false),
                    ErrorStagedUncommittedFiles = (changes.stagedUncommittedFiles != false),
                    ErrorWorkingTreeChanges = (changes.workingtreeChanges != false),
                    ErrorUnpushedCommitsOnMainBranch = (unpushedCommitsOnMain.result != false),
                    ErrorRebaseInProgress = (rebasing.result != false),
                    ErrorMergeInProgress = (merging.result != false),
                };
            }

            string restorebranch = string.Empty;
            if (currentbranch.shortname != MainBranch)
            {
                var checkout = new Command.Branch.ChangeBranchTo(this, MainBranch);
                checkout.WaitFor();

                var branch = new Command.Branch.GetCurrentBranch(this);
                branch.WaitFor();

                if (branch.shortname != MainBranch)
                {
                    return new BuildCacheAndCommitOnMainBranchResult()
                    {
                        ErrorChangingToMainBranch = true,
                    };
                }

                restorebranch = currentbranch.shortname;
            }

            try
            {
                var build = ProjectType.BuildCache(Executor);
                if (!build.Result)
                {
                    restorebranch = null; //Can't restore current branch as there are working tree changes or staged uncommitted files

                    return new BuildCacheAndCommitOnMainBranchResult()
                    {
                        BuildCache = build,
                        ErrorBuildingCache = true,
                    };
                }

                changes = new Command.WorkingTree.HasChanges(this);
                unpushedCommitsOnMain = new Command.Branch.HasBranchUnpushedChanges(this, MainBranch, null);

                changes.WaitFor();
                unpushedCommitsOnMain.WaitFor();
                if (changes.workingtreeChanges != true && changes.stagedUncommittedFiles != true && unpushedCommitsOnMain.result != true)
                {
                    return new BuildCacheAndCommitOnMainBranchResult()
                    {
                        NotUpdatedBecauseNothingChanged = true,
                        BuildCache = build,
                    };
                }

                bool updated = (unpushedCommitsOnMain.result == true);
                if (changes.workingtreeChanges == true || changes.stagedUncommittedFiles == true)
                {
                    var commit = new Command.Branch.CommitAllChanges(this, CommitMessage);
                    commit.WaitFor();
                    if (String.IsNullOrWhiteSpace(commit.commitid))
                    {
                        changes = new Command.WorkingTree.HasChanges(this);
                        changes.WaitFor();

                        if (changes.workingtreeChanges == true || changes.stagedUncommittedFiles == true)
                        {
                            restorebranch = null; //Can't restore current branch as there are working tree changes or staged uncommitted files

                            return new BuildCacheAndCommitOnMainBranchResult()
                            {
                                ErrorCommittingChanges = true,
                                BuildCache = build,
                            };
                        }
                    }
                    else
                    {
                        updated = true;
                    }
                }

                return new BuildCacheAndCommitOnMainBranchResult()
                {
                    Updated = updated,
                    NotUpdatedBecauseNothingChanged = !updated,
                    BuildCache = build,
                };
            }
            finally
            {
                if (restorebranch != null)
                {
                    var checkout = new Command.Branch.ChangeBranchTo(this, restorebranch);
                    checkout.WaitFor();
                }
            }
        }

        public void DeleteAllWorkingTreeChangesAndStagedUncommittedFiles_ResetMainBranchToRemote_CheckoutBranch(string CheckoutBranch)
        {
            {
                var changes = new Command.WorkingTree.DeleteAllWorkingTreeChangesAndStagedUncommittedFiles(this);
                changes.WaitFor();
            }

            {
                var checkout = new Command.Branch.ChangeBranchTo(this, MainBranch);
                checkout.WaitFor();

                var reset = new Command.Branch.ResetCurrentBranchToRemote(this);
                reset.WaitFor();
            }

            {
                var checkout = new Command.Branch.ChangeBranchTo(this, CheckoutBranch);
                checkout.WaitFor();
            }
        }


        public static void ClearCache()
        {
            Command.WorkingTree.IsGitRootDirectory.ClearCache();
            Command.WorkingTree.ResolveGitPath.ClearCache();
        }

        public static void LaunchDebugger()
        {
            System.Diagnostics.Debugger.Launch();
        }
    }
}
