using GitNoob.GitResult;
using System;
using System.IO;

namespace GitNoob.Git
{
    public partial class GitWorkingDirectory
    {
        private GetLatestResult GetLatestViaInit()
        {
            var initcmd = new Command.Repository.Init(this, MainBranch);
            initcmd.WaitFor();

            ClearCache();

            var rootdir = new Command.WorkingTree.IsGitRootDirectory(this, false);
            rootdir.WaitFor();
            if (rootdir.result != true)
            {
                throw new Exception("Error git initializing directory: " + _workingdirectory.Path);
            }

            var commitname = new Command.Config.GetCurrentCommitter(this);
            commitname.WaitFor();

            return new GetLatestResult
            {
                Initialized = true,
                CommitFullName = commitname.result,
                CommitName = commitname.name,
                CommitEmail = commitname.email,
            };
        }

        private GetLatestResult GetLatestViaNoGitNoobRemoteUrl()
        {
            var changes = new Command.WorkingTree.HasChanges(this);
            var currentbranch = new Command.Branch.GetCurrentBranch(this);
            changes.WaitFor();
            currentbranch.WaitFor();

            Command.Branch.HasBranchUnpushedChanges unpushedCommits;
            if (currentbranch.shortname != MainBranch)
            {
                unpushedCommits = new Command.Branch.HasBranchUnpushedChanges(this, currentbranch.shortname, MainBranch);
                unpushedCommits.WaitFor();
            }
            else
            {
                unpushedCommits = null;
            }

            bool CurrentBranchIsBehindMainBranch;
            var commitname = new Command.Config.GetCurrentCommitter(this);
            if (currentbranch.DetachedHead == true)
            {
                // Detached head can never be behind mainbranch.
                CurrentBranchIsBehindMainBranch = false;
            }
            else
            {
                if (MainBranch == currentbranch.shortname)
                {
                    // Mainbranch can never be behind itself (when RemoteUrl is empty)
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
            }
            commitname.WaitFor();

            return new GetLatestResult
            {
                NothingToUpdate_HasNoGitNoobRemoteUrl = true,

                CurrentBranch = currentbranch.shortname,
                DetachedHead_NotOnBranch = (currentbranch.DetachedHead == true),
                WorkingTreeChanges = (changes.workingtreeChanges != false),
                StagedUncommittedFiles = (changes.stagedUncommittedFiles != false),
                UnpushedCommits = (unpushedCommits != null && unpushedCommits.result == true),
                CurrentBranchIsBehindMainBranch = CurrentBranchIsBehindMainBranch,
                CommitFullName = commitname.result,
                CommitName = commitname.name,
                CommitEmail = commitname.email,
            };
        }


        private GetLatestResult GetLatestViaClone()
        {
            var clonecmd = new Command.Repository.Clone(this, MainBranch);
            clonecmd.WaitFor();

            ClearCache();

            var rootdir = new Command.WorkingTree.IsGitRootDirectory(this, false);
            rootdir.WaitFor();
            if (rootdir.result != true)
            {
                try
                {
                    Directory.Delete(_workingdirectory.Path.ToString(), true);
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
            if (currentbranch.DetachedHead == true)
            {
                // Detached head can never be behind mainbranch.
                CurrentBranchIsBehindMainBranch = false;
            }
            else
            {
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
            }
            commitname.WaitFor();

            return new GetLatestResult()
            {
                Updated = true,
                CurrentBranch = currentbranch.shortname,
                DetachedHead_NotOnBranch = (currentbranch.DetachedHead == true),
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
            if (!Directory.Exists(_workingdirectory.Path.ToString()))
            {
                try
                {
                    Directory.CreateDirectory(_workingdirectory.Path.ToString());
                    if (!Directory.Exists(_workingdirectory.Path.ToString()))
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
                clone = IsDirectoryEmpty(_workingdirectory.Path.ToString());
            }

            if (string.IsNullOrWhiteSpace(_workingdirectory.Git.RemoteUrl))
            {
                if (clone)
                {
                    return GetLatestViaInit();
                }
                else
                {
                    return GetLatestViaNoGitNoobRemoteUrl();
                }
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
    }
}
