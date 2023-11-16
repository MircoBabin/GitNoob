using System;

namespace GitNoob.GitResult
{
    public class RemoveLastTemporaryCommitOnCurrentBranchResult : BaseGitDisasterResult
    {
        public bool Removed { get; set; }
        public bool NoCommitToRemove { get; set; }

        public bool ErrorUnpacking { get; set; }

        public RemoveLastTemporaryCommitOnCurrentBranchResult() : base()
        {
            Removed = false;
            NoCommitToRemove = false;

            ErrorUnpacking = false;
        }
    }
}
