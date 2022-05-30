namespace GitNoob.Git
{
    public class GitCommit
    {
        public string CommitId { get; set; }
        public string Message { get; set; }

        public GitCommit(string CommitId, string Message)
        {
            this.CommitId = CommitId;
            this.Message = Message;
        }
    }
}
