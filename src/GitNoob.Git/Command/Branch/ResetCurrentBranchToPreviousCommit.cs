namespace GitNoob.Git.Command.Branch
{
    public class ResetCurrentBranchToPreviousCommit : Command
    {
        public ResetCurrentBranchToPreviousCommit(GitWorkingDirectory gitworkingdirectory) : base(gitworkingdirectory)
        {
            var reset = RunGit("reset", "reset --soft \"HEAD^\"");
            reset.WaitFor();

            var unstage = RunGit("unstage", "restore --staged .");
        }

        protected override void RunGitDone()
        {
        }
    }
}
