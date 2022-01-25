namespace GitNoob.Git.Command.Branch
{
    public class MergeAbort : Command
    {
        //public bool? result { get; private set; }

        public MergeAbort(GitWorkingDirectory gitworkingdirectory) : base(gitworkingdirectory)
        {
            //result = null;

            RunGit("merge", "merge --abort");
        }

        protected override void RunGitDone()
        {
            var executor = GetGitExecutor("merge");
            //result can't be determined
        }
    }
}
