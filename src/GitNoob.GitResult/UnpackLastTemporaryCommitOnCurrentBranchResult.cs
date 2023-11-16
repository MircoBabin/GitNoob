using System;

namespace GitNoob.GitResult
{
    public class UnpackLastCommitOnCurrentBranchResult : BaseGitDisasterResult
    {
        public bool Unpacked { get; set; }
        public bool NoCommitToUnpack { get; set; }

        public bool ErrorUnpacking { get; set; }

        public UnpackLastCommitOnCurrentBranchResult() : base()
        {
            Unpacked = false;
            NoCommitToUnpack = false;

            ErrorUnpacking = false;
        }
    }
}
