using GitNoob.Git.Result;
using System;

namespace GitNoob.Git
{
    /* Logical lock for a remote git branch. To prevent a fellow worker from updating the remote branch while you are
     * in a, lengthy like hour(s), rebase operation. Otherwise after your rebase, the push will fail because non-fast-forward. 
     * And then you have to start over rebasing. etc. 
     * 
     * Which is annoying and can be prevented by signalling your fellow workers you are busy integrating your changes to 
     * remote (GitHub, BitBucket, ...) and they must wait until you are done. This logical lock is an automated signalling mechanism.
     * 
     * Off course, the fellow worker will have to use GitNoob or another program that checks for the logical lock.
     * 
     * Because the logical lock has to be explicitly released, there is a Reset() to forcefully release the lock. In case GitNoob
     * acquired the lock but did not release (program crash, forced shutdown of computer, power outage, ...) this is a mechanism 
     * to cleanup a stale lock.
     * 
     * -----------------------------------------------------------------------------------------------------------------------------
     * Using git behaviour:
     * 
     * If you delete the tag locally, then create it pointing to a new place, then push, 
     * your push transfers the tag, and the remote sees this as a tag-change and rejects the change 
     * (because the commit where the tag to points changed), unless it's a force-push.
     * 
     * -----------------------------------------------------------------------------------------------------------------------------
     * Algorithm:
     * 
     * 1) Determine the annotated tagname "gitlock-<remoteBranchName-short-name>" (is done in constructor).
     * 2) Create an local unique commit on local last commit of <branchName>.
     * 3) Create the annotated tag on the local unique commit with lock-message ( see BuildLockMessage() ).
     * 4) Push the annotated tag pointing to remote. 
     *    The remote will accept if the tag does not exist.
     *    The remote will reject if the tag already exists, because the existing tag will be pointing to some other commit.
     * 5) Retrieve the annotated tag from remote.
     *    -Success: If the tag points to the same local unique commit and has the same message, the lock succeeded.
     *    -Failure: If not, the remote rejected the push tag, the lock failed.
     *              The lock is being held by someone else, inspect annotated tag message for information ( see SplitLockMessage() ).
     *              
     *              
     *              
     * The lock-message is constructed as:
     *     OCTET  = <any 8-bit sequence of data>
     *     CHAR   = <any US-ASCII character (octets 0 - 127)>
     *     SP     = <US-ASCII SP, space (32)>
     *     BASE64 = rfc4648 - BASE 64 encoding
     *     UTF8   = rfc3629 - UTF-8 encoding
     *     
     *     "gitlock" SP+ "[" username "]" SP+ "[" time "] SP+ "[" randomsha1 "]" SP+ "[" message "]" CHAR*
     *     
     *     username = BASE64( UTF8("name of user who locked, can be the git config values 'user.name <user.email>'") )
     *     
     *     time = year "-" month "-" day "T" hour ":" minute ":" second "Z"
     *         # Contains UTC time of constructing this lock message. e.g. "2021-08-13T01:03:05Z"
     *         year   = [0-9][0-9][0-9][0-9]
     *         month  = [0-9][0-9]           with range 01 upto and including 12
     *         day    = [0-9][0-9]           with range 01 upto and including 31
     *         hour   = [0-9][0-9]           with range 00 upto and including 23
     *         minute = [0-9][0-9]           with range 00 upto and including 59
     *         second = [0-9][0-9]           with range 00 upto and including 59
     *     
     *     randomsha1 = BASE64( UTF8("A random sha1 hash in hexadecimal format, so 40 characters long.
     *                                Generated randomly when constructing this lock message.
     *                                Must not be a git commitid or other git sha1 value. 
     *                                Must not be based on time creating this lock message.") ) 
     *         # When trying to acquire a lock from multiple programs by the same user (on different computers or the same computer)
     *         # on exactly the same second, make sure only one succeeds because of the randomness.
     *         
     *     message = BASE64( UTF8("I'm integrating. Please wait until I'm finished. Or some other message.") ) 
     */

    public sealed class GitLock
    {
        public GitWorkingDirectory gitworkingdirectory { get; private set; }

        public string branchName { get; private set; }
        public string remoteName { get; private set; }
        public string remoteBranchName { get; private set; }
        public string lockTagName { get; private set; }

        private bool acquired = false;
        private GitTag acquiredTag = null;

        public GitLock(GitWorkingDirectory gitworkingdirectory, string branchName)
        {
            this.gitworkingdirectory = gitworkingdirectory;
            this.branchName = branchName;

            {
                var remote = new Command.Branch.GetRemoteBranch(gitworkingdirectory, branchName);
                remote.WaitFor();
                if (string.IsNullOrEmpty(remote.remoteName) || string.IsNullOrEmpty(remote.remoteBranch))
                    throw new Exception("Branch " + branchName + " is not a remote tracking branch.");

                this.remoteName = remote.remoteName;
                this.remoteBranchName = remote.remoteBranch;
            }

            this.lockTagName = "gitlock-" + this.remoteBranchName;
        }

        private string BuildLockMessage(string username, string randomsha1, string message)
        {
            return "gitlock" +
                   " [" + GitUtils.EncodeUtf8Base64(username) + "]" +
                   " [" + DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'") + "]" +
                   " [" + GitUtils.EncodeUtf8Base64(randomsha1) + "]" +
                   " [" + GitUtils.EncodeUtf8Base64(message) + "]";
        }

        private void SplitLockMessage(string lockmessage, out string username, out DateTime? lockedtime, out string randomsha1, out string message)
        {
            username = String.Empty;
            lockedtime = null;
            randomsha1 = String.Empty;
            message = String.Empty;

            var parts = lockmessage.Split('[');
            int partno = 0;
            foreach (var part in parts)
            {
                var value = part.Trim();
                if (value.EndsWith("]") && value.Length > 1)
                {
                    value = value.Substring(0, value.Length - 1).Trim();

                    partno++;
                    switch (partno)
                    {
                        case 1:
                            username = GitUtils.DecodeUtf8Base64(value);
                            break;

                        case 2:
                            try
                            {
                                lockedtime = DateTime.Parse(value);
                            }
                            catch { }
                            break;

                        case 3:
                            randomsha1 = GitUtils.DecodeUtf8Base64(value);
                            break;

                        case 4:
                            message = GitUtils.DecodeUtf8Base64(value);
                            break;
                    }
                }
            }
        }

        private GitTag GetLockTag()
        {
            var list = new Command.Tag.ListTags(gitworkingdirectory);
            list.WaitFor();

            GitTag locktag = null;
            foreach (var tag in list.result)
            {
                if (tag.Value.ShortName == lockTagName)
                {
                    locktag = tag.Value;
                    break;
                }
            }

            return locktag;
        }

        public GitLockResult Acquire(string username, string message = null, bool CheckGitCredentialsViaKeePassCommander = true)
        {
            var currentbranch = new Command.Branch.GetCurrentBranch(gitworkingdirectory);
            var changes = new Command.WorkingTree.HasChanges(gitworkingdirectory);
            var rebasing = new Command.WorkingTree.IsRebaseActive(gitworkingdirectory);
            var merging = new Command.WorkingTree.IsMergeActive(gitworkingdirectory);
            currentbranch.WaitFor();
            changes.WaitFor();
            rebasing.WaitFor();
            merging.WaitFor();

            if (changes.workingtreeChanges != false || changes.stagedUncommittedFiles != false || rebasing.result != false || merging.result != false)
            {
                return new GitLockResult()
                {
                    ErrorStagedUncommittedFiles = (changes.stagedUncommittedFiles != false),
                    ErrorWorkingTreeChanges = (changes.workingtreeChanges != false),
                    ErrorRebaseInProgress = (rebasing.result != false),
                    ErrorMergeInProgress = (merging.result != false),
                };
            }

            /* 0. Initialize */
            string randomsha1 = GitUtils.GenerateRandomSha1();
            string tempbranch = "gitlock-branch-" + randomsha1;
            string rawlockmessage = BuildLockMessage(username, randomsha1, message);

            GitTag locktag = null;
            bool locktagPushError = false;
            string locktagPushoutput = String.Empty;

            /* 1. Checkout branchName to create a new local unique commit */
            {
                var change = new Command.Branch.ChangeBranchTo(gitworkingdirectory, branchName);
                change.WaitFor();

                var branch = new Command.Branch.GetCurrentBranch(gitworkingdirectory);
                branch.WaitFor();
                if (branch.shortname != branchName)
                {
                    return new GitLockResult()
                    {
                        ErrorCreatingEmptyLockCommit = true,
                    };
                }
            }

            try
            {
                /* 2. Create new temporary branch for local unique commit */
                {
                    var create = new Command.Branch.CreateBranchFromCurrentBranch(gitworkingdirectory, tempbranch);
                    create.WaitFor();

                    var change = new Command.Branch.ChangeBranchTo(gitworkingdirectory, tempbranch);
                    change.WaitFor();

                    var branch = new Command.Branch.GetCurrentBranch(gitworkingdirectory);
                    branch.WaitFor();
                    if (branch.shortname != tempbranch)
                    {
                        return new GitLockResult()
                        {
                            ErrorCreatingEmptyLockCommit = true,
                        };
                    }
                }

                try
                {
                    /* 3. Create new local unique commit on temporary branch*/
                    {
                        var commit = new Command.Branch.CreateEmptyCommitOnCurrentBranch(gitworkingdirectory, rawlockmessage);
                        commit.WaitFor();
                    }

                    /* 4. Create locktag to new local unique commit*/
                    {
                        var delete = new Command.Tag.DeleteLocalTag(gitworkingdirectory, lockTagName);
                        delete.WaitFor();

                        var create = new Command.Tag.CreateTagToLastCommitOnCurrentBranch(gitworkingdirectory, lockTagName, rawlockmessage);
                        create.WaitFor();

                        locktag = GetLockTag();
                        if (locktag == null || locktag.Message != rawlockmessage)
                        {
                            return new GitLockResult()
                            {
                                ErrorCreatingLockTag = true,
                            };
                        }
                    }

                    /* pre 5 Check remote reachable */
                    if (CheckGitCredentialsViaKeePassCommander && !GitCredentialsViaKeePassCommander.AreCredentialsAvailable(gitworkingdirectory))
                    {
                        return new GitLockResult
                        {
                            ErrorKeePassNotStarted = true,
                        };
                    }

                    var reachable = new Command.Repository.RemoteReachable(gitworkingdirectory);
                    reachable.WaitFor();
                    if (reachable.result != true)
                    {
                        return new GitLockResult
                        {
                            ErrorRemoteNotReachable = true,
                        };
                    }

                    /* 5 Push locktag to remote */
                    {
                        /* push will fail if the lockTagName already exists, because the new sha1 is different */
                        var push = new Command.Tag.PushTagToRemote(gitworkingdirectory, remoteName, lockTagName);
                        push.WaitFor();

                        if (push.result != true)
                        {
                            locktagPushError = true;
                            locktagPushoutput = push.output;
                        }
                    }

                    /* 6. Fetch locktag from remote */
                    {
                        var delete = new Command.Tag.DeleteLocalTag(gitworkingdirectory, lockTagName);
                        delete.WaitFor();

                        var fetch = new Command.Tag.FetchTagFromRemote(gitworkingdirectory, remoteName, lockTagName);
                        fetch.WaitFor();
                    }
                }
                finally
                {
                    /* Delete temporary branch, created in 2.*/
                    var change = new Command.Branch.ChangeBranchTo(gitworkingdirectory, currentbranch.shortname);
                    change.WaitFor();

                    var delete = new Command.Branch.DeleteBranch(gitworkingdirectory, tempbranch, true);
                    delete.WaitFor();
                }

                /* 7. Compare fetched locktag with created gittag on commitid & message*/
                {
                    var list = new Command.Tag.ListTags(gitworkingdirectory);
                    list.WaitFor();

                    GitTag remotetag = GetLockTag();
                    if (remotetag == null || remotetag.Commit != locktag.Commit || remotetag.Message != locktag.Message)
                    {
                        string lockedby = String.Empty;
                        DateTime? lockedtime = null;
                        string lockedrandomsha1 = String.Empty;
                        string lockedmessage = String.Empty;
                        if (remotetag != null)
                        {
                            SplitLockMessage(remotetag.Message, out lockedby, out lockedtime, out lockedrandomsha1, out lockedmessage);
                        }

                        return new GitLockResult()
                        {
                            ErrorCreatingLockTag = true,

                            ErrorPushingLockTag = locktagPushError,
                            PushOutput = locktagPushoutput,

                            LockedTime = lockedtime,
                            LockedBy = lockedby,
                            LockedMessage = lockedmessage,

                            GitLock = this,
                        };
                    }
                }
            }
            finally
            {
                /* Restore current branch, changed in 1. */
                var change = new Command.Branch.ChangeBranchTo(gitworkingdirectory, currentbranch.shortname);
                change.WaitFor();
            }

            /* 8. Success */
            acquired = true;
            acquiredTag = locktag;

            return new GitLockResult()
            {
                Locked = true,
                GitLock = this,
            };
        }

        public GitLockResult Release(bool CheckGitCredentialsViaKeePassCommander = true)
        {
            if (!acquired)
            {
                return new GitLockResult()
                {
                    ErrorLockNotAcquired = true,
                };
            }

            if (CheckGitCredentialsViaKeePassCommander && !GitCredentialsViaKeePassCommander.AreCredentialsAvailable(gitworkingdirectory))
            {
                return new GitLockResult
                {
                    ErrorKeePassNotStarted = true,
                };
            }

            var reachable = new Command.Repository.RemoteReachable(gitworkingdirectory);
            reachable.WaitFor();
            if (reachable.result != true)
            {
                return new GitLockResult
                {
                    ErrorRemoteNotReachable = true,
                };
            }

            var delete = new Command.Tag.DeleteLocalTag(gitworkingdirectory, lockTagName);
            delete.WaitFor();

            var fetch = new Command.Tag.FetchTagFromRemote(gitworkingdirectory, remoteName, lockTagName);
            fetch.WaitFor();

            var remotetag = GetLockTag();
            if (remotetag == null || remotetag.Commit != acquiredTag.Commit || remotetag.Message != acquiredTag.Message)
            {
                string lockedby = String.Empty;
                DateTime? lockedtime = null;
                string lockedrandomsha1 = String.Empty;
                string lockedmessage = String.Empty;
                if (remotetag != null)
                {
                    SplitLockMessage(remotetag.Message, out lockedby, out lockedtime, out lockedrandomsha1, out lockedmessage);
                }

                return new GitLockResult()
                {
                    ErrorLockNotOwned = true,

                    LockedTime = lockedtime,
                    LockedBy = lockedby,
                    LockedMessage = lockedmessage,
                };
            }

            return Reset(false, false);
        }

        public GitLockResult Reset(bool CheckGitCredentialsViaKeePassCommander = true, bool CheckRemoteReachable = true)
        {
            //Hard reset lock, doesn't check if the lock is owned

            if (CheckGitCredentialsViaKeePassCommander && !GitCredentialsViaKeePassCommander.AreCredentialsAvailable(gitworkingdirectory))
            {
                return new GitLockResult
                {
                    ErrorKeePassNotStarted = true,
                };
            }

            if (CheckRemoteReachable)
            {
                var reachable = new Command.Repository.RemoteReachable(gitworkingdirectory);
                reachable.WaitFor();
                if (reachable.result != true)
                {
                    return new GitLockResult
                    {
                        ErrorRemoteNotReachable = true,
                    };
                }
            }

            var delete = new Command.Tag.DeleteLocalTag(gitworkingdirectory, lockTagName);
            delete.WaitFor();

            var remote = new Command.Tag.DeleteRemoteTag(gitworkingdirectory, remoteName, lockTagName);
            remote.WaitFor();

            acquired = false;
            acquiredTag = null;

            return new GitLockResult()
            {
                Unlocked = (remote.result == true),

                ErrorPushingLockTag = (remote.result != true),
                PushOutput = remote.output,
            };
        }
    }
}
