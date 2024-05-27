namespace GitNoob.Git.Command.Branch
{
    public class ResetCurrentBranchToPreviousCommit : Command
    {
        public ResetCurrentBranchToPreviousCommit(GitWorkingDirectory gitworkingdirectory, bool putLastCommitChangesInWorkingDirectory) : base(gitworkingdirectory)
        {
            if (putLastCommitChangesInWorkingDirectory)
            {
                var reset = RunGit("reset", "reset --soft \"HEAD^\"");
                reset.WaitFor();

                var unstage = RunGit("unstage", "restore --staged .");
                unstage.WaitFor();
            }
            else
            {
                var reset = RunGit("reset", "reset --hard \"HEAD^\"");
                reset.WaitFor();
            }
        }

        protected override void RunGitDone()
        {
        }
    }
}
