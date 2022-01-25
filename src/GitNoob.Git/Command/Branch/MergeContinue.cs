namespace GitNoob.Git.Command.Branch
{
    public class MergeContinue : Command
    {
        //public bool? result { get; private set; }

        public MergeContinue(GitWorkingDirectory gitworkingdirectory) : base(gitworkingdirectory)
        {
            //result = null;

            RunGit("merge", "merge --continue");
        }

        protected override void RunGitDone()
        {
            var executor = GetGitExecutor("merge");
            //result can't be determined
        }
    }
}
