using System;

namespace GitNoob.GitResult
{
    public class CreateNewBranchResult : BaseGitDisasterResult
    {
        public bool Created { get; set; }
        public string CurrentBranch { get; set; }

        public bool ErrorBranchAlreadyExists { get; set; }
        public bool ErrorCreating { get; set; }

        public bool TemporaryCommitFailed { get; set; }
        public CommitAllChangesOnCurrentBranchResult TemporaryCommitResult { get; set; }

        public CreateNewBranchResult()
        {
            Created = false;
            CurrentBranch = String.Empty;

            ErrorBranchAlreadyExists = false;
            ErrorCreating = false;

            TemporaryCommitFailed = false;
            TemporaryCommitResult = null;
        }
    }
}
