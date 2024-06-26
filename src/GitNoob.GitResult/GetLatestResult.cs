﻿using System;

namespace GitNoob.GitResult
{
    public class GetLatestResult
    {
        public bool ErrorKeePassNotStarted { get; set; }
        public bool ErrorRemoteNotReachable { get; set; }

        public bool Initialized { get; set; }

        public bool Cloned { get; set; }
        public bool ErrorNonEmptyAndNotAGitRepository { get; set; }

        public bool Updated { get; set; }
        public string CurrentBranch { get; set; }
        public bool DetachedHead_NotOnBranch { get; set; }
        public bool WorkingTreeChanges { get; set; }
        public bool StagedUncommittedFiles { get; set; }
        public bool UnpushedCommits { get; set; }
        public bool CurrentBranchIsBehindMainBranch { get; set; }
        public string CommitFullName { get; set; }
        public string CommitName { get; set; }
        public string CommitEmail { get; set; }

        public bool NothingToUpdate_HasNoGitNoobRemoteUrl { get; set; }

        public bool ErrorStagedUncommittedFiles { get; set; }
        public bool ErrorUnpushedCommitsOnMainBranch { get; set; }
        public bool ErrorUnpushedCommitsAndOnLocalTrackingRemoteBranch { get; set; } //If current-branch == main-branch this will be true
        public bool ErrorWorkingTreeChangesAndOnLocalTrackingRemoteBranch { get; set; }

        public GetLatestResult()
        {
            ErrorKeePassNotStarted = false;
            ErrorRemoteNotReachable = false;

            Initialized = false;

            Cloned = false;
            ErrorNonEmptyAndNotAGitRepository = false;

            Updated = false;
            CurrentBranch = String.Empty;
            DetachedHead_NotOnBranch = false;
            WorkingTreeChanges = false;
            StagedUncommittedFiles = false;
            UnpushedCommits = false;
            CurrentBranchIsBehindMainBranch = false;
            CommitFullName = string.Empty;
            CommitName = string.Empty;
            CommitEmail = string.Empty;

            NothingToUpdate_HasNoGitNoobRemoteUrl = false;

            ErrorStagedUncommittedFiles = false;
            ErrorUnpushedCommitsOnMainBranch = false;
            ErrorUnpushedCommitsAndOnLocalTrackingRemoteBranch = false;
            ErrorWorkingTreeChangesAndOnLocalTrackingRemoteBranch = false;
        }
    }
}
