namespace GitNoob.Git.Command.Branch
{
    public class MergeFastForwardOnly : Command
    {
        public bool? result { get; private set; }

        //Warning: checked out branch is intoBranch (e.g. "main") afterwards !
        public MergeFastForwardOnly(GitWorkingDirectory gitworkingdirectory, string fromBranch, string intoBranch) : base(gitworkingdirectory)
        {
            result = null;

            var change = new ChangeBranchTo(gitworkingdirectory, intoBranch);
            change.WaitFor();
            if (change.result != true)
            {
                result = false;
                return;
            }

            RunGit("merge", new string[] { "merge", "--ff-only", fromBranch });
        }

        protected override void RunGitDone()
        {
            if (result == null)
            {
                var executor = GetGitExecutor("merge");
                result = (executor.ExitCode == 0);
            }
        }
    }
}
