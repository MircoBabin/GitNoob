namespace GitNoob.Git.Command.Branch
{
    public class RebaseContinue : Command
    {
        //public bool? result { get; private set; }

        public RebaseContinue(GitWorkingDirectory gitworkingdirectory) : base(gitworkingdirectory)
        {
            //result = null;

            RunGit("rebase", "rebase --continue");
        }

        protected override void RunGitDone()
        {
            var executor = GetGitExecutor("rebase");
            //result can't be determined
        }
    }
}
