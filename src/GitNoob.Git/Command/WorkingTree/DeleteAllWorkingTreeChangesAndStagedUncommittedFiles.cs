namespace GitNoob.Git.Command.WorkingTree
{
    public class DeleteAllWorkingTreeChangesAndStagedUncommittedFiles : Command
    {
        public DeleteAllWorkingTreeChangesAndStagedUncommittedFiles(GitWorkingDirectory gitworkingdirectory) : base(gitworkingdirectory)
        {
            {
                var execute = RunGit("workingtree-staged", "reset --hard");
                execute.WaitFor();
            }

            RunGit("new-files", "clean --force -d --quiet");
        }

        protected override void RunGitDone()
        {
        }
    }
}
