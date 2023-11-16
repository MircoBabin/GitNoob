using System;

namespace GitNoob.GitResult
{
    public class HasGitNoobTemporaryCommitResult : BaseGitDisasterResult
    {
        public bool HasGitNoobTemporaryCommit { get; set; }
        public bool HasNoGitNoobTemporaryCommit { get; set; }
        public uint NumberOfGitNoobTemporaryCommits { get; set; }

        public bool ErrorNoCommonCommitWithMainBranch { get; set; }

        public HasGitNoobTemporaryCommitResult() : base()
        {
            HasGitNoobTemporaryCommit = false;
            HasNoGitNoobTemporaryCommit = false;
            NumberOfGitNoobTemporaryCommits = 0;

            ErrorNoCommonCommitWithMainBranch = false;
        }
    }
}
