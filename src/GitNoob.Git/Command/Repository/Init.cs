namespace GitNoob.Git.Command.Repository
{
    public class Init : Command
    {
        //public string result { get; private set; }

        public Init(GitWorkingDirectory gitworkingdirectory, string mainBranchName) : base(gitworkingdirectory)
        {
            //result = null;

            RunGit("init", new string[] { "init",
                (!string.IsNullOrWhiteSpace(mainBranchName) ? "--initial-branch=" + mainBranchName : "--no-checkout") });
        }

        protected override void RunGitDone()
        {
            var init = GetGitExecutor("init");
            //result can't be determined
        }
    }
}
