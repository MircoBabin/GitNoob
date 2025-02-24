namespace GitNoob.Git.Command.WorkingTree
{
    public class DeleteAllWorkingTreeChangesAndStagedUncommittedFiles : Command
    {
        public DeleteAllWorkingTreeChangesAndStagedUncommittedFiles(GitWorkingDirectory gitworkingdirectory) : base(gitworkingdirectory)
        {
            {
                var execute = RunGit("workingtree-staged", new string[] { "reset", "--hard" });
                execute.WaitFor();
            }

            RunGit("new-files", new string[] { "clean", "--force", "-d", "--quiet" });
        }

        protected override void RunGitDone()
        {
        }
    }
}
