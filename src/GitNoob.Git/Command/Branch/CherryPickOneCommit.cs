namespace GitNoob.Git.Command.Branch
{
    public class CherryPickOneCommit : Command
    {
        //public bool? result { get; private set; }

        public CherryPickOneCommit(GitWorkingDirectory gitworkingdirectory, string commitid) : base(gitworkingdirectory)
        {
            //result = null;

            RunGit("cherry-pick", new string[] { "cherry-pick", "--cleanup=verbatim", commitid });
        }

        protected override void RunGitDone()
        {
            var executor = GetGitExecutor("cherry-pick");
            //result can not be determined
        }
    }
}
