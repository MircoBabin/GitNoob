using System;

namespace GitNoob.GitResult
{
    public class ChangeCurrentBranchResult : BaseGitDisasterResult
    {
        public bool Changed { get; set; }
        public string CurrentBranch { get; set; }

        public bool ErrorChanging { get; set; }

        public bool TemporaryCommitFailed { get; set; }
        public CommitAllChangesOnCurrentBranchResult TemporaryCommitResult { get; set; }

        public ChangeCurrentBranchResult() : base()
        {
            Changed = false;
            CurrentBranch = String.Empty;

            ErrorChanging = false;

            TemporaryCommitFailed = false;
            TemporaryCommitResult = null;
        }
    }
}
