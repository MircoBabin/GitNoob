namespace GitNoob.GitResult
{
    public class DeleteCurrentBranchResult : BaseGitDisasterResult
    {
        public bool Deleted { get; set; }

        public bool ErrorCurrentBranchHasChanged { get; set; }
        public bool ErrorCannotDeleteMainBranch { get; set; }
        public bool ErrorCreatingSafetyTag { get; set; }
        public bool ErrorDeleting { get; set; }

        public DeleteCurrentBranchResult() : base()
        {
            Deleted = false;

            ErrorCurrentBranchHasChanged = false;
            ErrorCannotDeleteMainBranch = false;
            ErrorCreatingSafetyTag = false;
            ErrorDeleting = false;
        }
    }
}
