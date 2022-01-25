namespace GitNoob.Git.Command.Branch
{
    public class CreateEmptyCommitOnCurrentBranch : Command
    {
        //public bool? result { get; private set; }

        public CreateEmptyCommitOnCurrentBranch(GitWorkingDirectory gitworkingdirectory, string message) : base(gitworkingdirectory)
        {
            //result = null;

            RunGit("commit", "commit --allow-empty --message \"" + message + "\"");
        }

        protected override void RunGitDone()
        {
            var executor = GetGitExecutor("commit");
            //result can not be determined
        }
    }
}
