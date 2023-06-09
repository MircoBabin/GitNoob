using System;

namespace GitNoob.GitResult
{
    public class HasGitNoobTemporaryCommitResult
    {
        public string CurrentBranch { get; set; }

        public bool HasGitNoobTemporaryCommit { get; set; }
        public bool HasNoGitNoobTemporaryCommit { get; set; }
        public uint NumberOfGitNoobTemporaryCommits { get; set; }

        public bool ErrorDetachedHead { get; set; }
        public bool ErrorNoCommonCommitWithMainBranch { get; set; }

        public HasGitNoobTemporaryCommitResult()
        {
            CurrentBranch = String.Empty;

            HasGitNoobTemporaryCommit = false;
            HasNoGitNoobTemporaryCommit = false;
            NumberOfGitNoobTemporaryCommits = 0;

            ErrorDetachedHead = false;
            ErrorNoCommonCommitWithMainBranch = false;
        }
    }
}
