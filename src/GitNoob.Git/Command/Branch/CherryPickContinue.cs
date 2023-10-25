namespace GitNoob.Git.Command.Branch
{
    public class CherryPickAbort : Command
    {
        //public bool? result { get; private set; }

        public CherryPickAbort(GitWorkingDirectory gitworkingdirectory) : base(gitworkingdirectory)
        {
            //result = null;

            RunGit("cherry-pick", "cherry-pick --abort");
        }

        protected override void RunGitDone()
        {
            var executor = GetGitExecutor("cherry-pick");
            //result can't be determined
        }
    }
}
