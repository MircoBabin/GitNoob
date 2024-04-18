namespace GitNoob.GitResult
{
    public class EnsureMainBranchExistanceResult : BaseGitDisasterResult
    {
        public bool Exists { get; set; }

        public bool ErrorLocalBranchNotFound { get; set; } //because RemoteUrl is empty and the local branch does not exist
        public bool ErrorRemoteBranchNotFound { get; set; } //because there is no remote branch named the same
        public bool ErrorCreatingMainBranch { get; set; } //because there is no remote branch named the same

        public EnsureMainBranchExistanceResult() : base()
        {
            Exists = false;

            ErrorLocalBranchNotFound = false;
            ErrorRemoteBranchNotFound = false;
            ErrorCreatingMainBranch = false;
        }
    }
}
