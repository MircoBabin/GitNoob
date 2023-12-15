namespace GitNoob.GitResult
{
    public class CheckForGitDisasterResult : BaseGitDisasterResult
    {
        public string CurrentBranchLastCommitId { get; set; }
        public string CurrentBranchLastCommitMessage { get; set; }

        public CheckForGitDisasterResult() : base()
        {
            CurrentBranchLastCommitId = string.Empty;
            GitDisaster_CurrentBranchShortName = string.Empty;
        }
    }
}
