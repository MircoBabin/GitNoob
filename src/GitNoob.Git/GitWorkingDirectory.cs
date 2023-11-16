using GitNoob.GitResult;
using GitNoob.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace GitNoob.Git
{
    public partial class GitWorkingDirectory
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
                return _workingdirectory.Path.ToString();
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
            if (!Directory.Exists(_workingdirectory.Path.ToString()))
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
                    ClearCommitNameAndEmailOnExit = ConfigWorkingDirectory.Git.ClearCommitNameAndEmailOnExit.Value,
                    TouchTimestampOfCommitsBeforeMerge = ConfigWorkingDirectory.Git.TouchTimestampOfCommitsBeforeMerge.Value,
                };
            }

            var currentbranch = new Command.Branch.GetCurrentBranch(this);
            var commitname = new Command.Config.GetCurrentCommitter(this);
            var changes = new Command.WorkingTree.HasChanges(this);
            var rebasing = new Command.WorkingTree.IsRebaseActive(this);
            var merging = new Command.WorkingTree.IsMergeActive(this);
            var cherrypicking = new Command.WorkingTree.IsCherryPickActive(this);
            var reverting = new Command.WorkingTree.IsRevertActive(this);
            var conflicts = new Command.WorkingTree.HasConflicts(this);
            var mainbranchcommit = new Command.Branch.GetLastCommitOfBranch(this, MainBranch);
            var mainbranchremote = new Command.Branch.GetRemoteBranch(this, MainBranch);

            currentbranch.WaitFor();
            var currentlastcommit = new Command.Branch.GetLastCommitOfBranch(this, currentbranch.shortname);
            currentlastcommit.WaitFor();

            commitname.WaitFor();
            changes.WaitFor();
            rebasing.WaitFor();
            merging.WaitFor();
            cherrypicking.WaitFor();
            reverting.WaitFor();
            conflicts.WaitFor();
            mainbranchcommit.WaitFor();
            mainbranchremote.WaitFor();

            return new StatusResult
            {
                DirectoryExists = true,
                IsGitRootDirectory = true,
                ClearCommitNameAndEmailOnExit = ConfigWorkingDirectory.Git.ClearCommitNameAndEmailOnExit.Value,
                TouchTimestampOfCommitsBeforeMerge = ConfigWorkingDirectory.Git.TouchTimestampOfCommitsBeforeMerge.Value,
                DetachedHead_NotOnBranch = (currentbranch.DetachedHead == true),
                CurrentBranch = currentbranch.shortname,
                CurrentBranchLastCommitId = currentlastcommit.commitid,
                CurrentBranchLastCommitMessage = currentlastcommit.commitmessage,
                CommitFullName = commitname.result,
                CommitName = commitname.name,
                CommitEmail = commitname.email,
                HasWorkingTreeChanges = (changes.workingtreeChanges == true), //modified, deleted and/or untracked files
                HasStagedUncommittedFiles = (changes.stagedUncommittedFiles == true),
                Rebasing = (rebasing.result == true),
                Merging = (merging.result == true),
                CherryPicking = (cherrypicking.result == true),
                Reverting = (reverting.result == true),
                Conflicts = (conflicts.result == true),
                MainBranchExists = (!string.IsNullOrEmpty(mainbranchcommit.commitid)),
                MainBranchIsTrackingRemoteBranch = (!string.IsNullOrEmpty(mainbranchremote.result)),
            };
        }

        public PruneResult PruneAggressive()
        {
            var result = new PruneResult();
            if (GitDisaster.Check(this, result))
                return result;

            {
                var command = new Command.Repository.PruneAggressive(this);
                command.WaitFor();
            }

            result.Pruned = true;

            return result;
        }

        public void DeleteTag(GitDeletedBranch deleted)
        {
            var result = new Command.Tag.DeleteLocalTag(this, deleted.Tag.ShortName);
            result.WaitFor();
        }

        public void DeleteTag(IEnumerable<GitDeletedBranch> deletions)
        {
            foreach (var deleted in deletions)
            {
                var result = new Command.Tag.DeleteLocalTag(this, deleted.Tag.ShortName);
                result.WaitFor();
            }
        }

        public GitLockResult AcquireGitLockForMainBranch(string username, string message = null)
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
            var result = new PushResult();
            if (GitDisaster.Check(this, result, new GitDisasterAllowed() { Allow_UnpushedCommitsOnMainBranch = true }))
                return result;

            if (CheckGitCredentialsViaKeePassCommander && !GitCredentialsViaKeePassCommander.AreCredentialsAvailable(this))
            {
                result.ErrorKeePassNotStarted = true;

                return result;
            }

            var reachable = new Command.Repository.RemoteReachable(this);
            reachable.WaitFor();
            if (reachable.result != true)
            {
                result.ErrorRemoteNotReachable = true;

                return result;
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
                    result.PushOutput = push.output;
                    result.ErrorConflicts = true;

                    return result;
                }
            }

            var unpushedCommitsOnMain = new Command.Branch.HasBranchUnpushedChanges(this, MainBranch, null);
            unpushedCommitsOnMain.WaitFor();

            if (unpushedCommitsOnMain.result != false)
            {
                result.ErrorStillUnpushedCommitsOnMainBranch = true;

                return result;
            }

            result.Pushed = true;
            return result;
        }

        public DeleteWorkingTreeChangesAndStagedUncommittedFilesResult DeleteWorkingTreeChangesAndStagedUncommittedFiles()
        {
            var result = new DeleteWorkingTreeChangesAndStagedUncommittedFilesResult();
            if (GitDisaster.Check(this, result, new GitDisasterAllowed() { Allow_StagedUncommittedFiles = true, Allow_WorkingTreeChanges = true }))
                return result;

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
                    result.ErrorStillStagedUncommittedFiles = (changes.stagedUncommittedFiles != false);
                    result.ErrorStillWorkingTreeChanges = (changes.workingtreeChanges != false);

                    return result;
                }
            }

            result.ChangesDeleted = true;
            return result;
        }

        public BuildCacheAndCommitOnMainBranchResult BuildCacheAndCommitOnMainBranch(Config.IExecutor Executor, string CommitMessage)
        {
            var result = new BuildCacheAndCommitOnMainBranchResult();
            if (GitDisaster.Check(this, result))
                return result;

            Config.IProjectType ProjectType = _workingdirectory.ProjectType;
            if (ProjectType == null ||
                !ProjectType.Capabilities.CapableOfClearAndBuildCache)
            {
                result.NotUpdatedBecauseNothingChanged = true;

                return result;
            }

            string restorebranch = string.Empty;
            if (result.GitDisaster_CurrentBranchShortName != MainBranch)
            {
                var checkout = new Command.Branch.ChangeBranchTo(this, MainBranch);
                checkout.WaitFor();

                var branch = new Command.Branch.GetCurrentBranch(this);
                branch.WaitFor();

                if (branch.shortname != MainBranch)
                {
                    result.ErrorChangingToMainBranch = true;

                    return result;
                }

                restorebranch = result.GitDisaster_CurrentBranchShortName;
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

                var changes = new Command.WorkingTree.HasChanges(this);
                var unpushedCommitsOnMain = new Command.Branch.HasBranchUnpushedChanges(this, MainBranch, null);

                changes.WaitFor();
                unpushedCommitsOnMain.WaitFor();
                if (changes.workingtreeChanges != true && changes.stagedUncommittedFiles != true && unpushedCommitsOnMain.result != true)
                {
                    result.NotUpdatedBecauseNothingChanged = true;
                    result.BuildCache = build;

                    return result;
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

                            result.ErrorCommittingChanges = true;
                            result.BuildCache = build;

                            return result;
                        }
                    }
                    else
                    {
                        updated = true;
                    }
                }

                result.Updated = updated;
                result.NotUpdatedBecauseNothingChanged = !updated;
                result.BuildCache = build;
                return result;
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
