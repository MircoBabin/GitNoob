namespace GitNoob.GitResult
{
    public class DeleteBranchResult : BaseGitDisasterResult
    {
        public bool Deleted { get; set; }

        public bool ErrorCannotDeleteMainBranch { get; set; }
        public bool ErrorChangingToMainBranch { get; set; }
        public bool ErrorCreatingSafetyTag { get; set; }
        public bool ErrorDeleting { get; set; }

        public DeleteBranchResult() : base()
        {
            Deleted = false;

            ErrorCannotDeleteMainBranch = false;
            ErrorChangingToMainBranch = false;
            ErrorCreatingSafetyTag = false;
            ErrorDeleting = false;
        }
    }
}
