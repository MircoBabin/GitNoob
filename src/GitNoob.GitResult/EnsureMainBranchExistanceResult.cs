namespace GitNoob.GitResult
{
    public class EnsureMainBranchExistanceResult
    {
        public bool Exists { get; set; }

        public bool ErrorRemoteBranchNotFound { get; set; } //because there is no remote branch named the same
        public bool ErrorCreatingMainBranch { get; set; } //because there is no remote branch named the same

        public EnsureMainBranchExistanceResult()
        {
            Exists = false;

            ErrorRemoteBranchNotFound = false;
            ErrorCreatingMainBranch = false;
        }
    }
}
