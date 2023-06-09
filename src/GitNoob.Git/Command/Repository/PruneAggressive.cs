namespace GitNoob.Git.Command.Repository
{
    public class PruneAggressive : Command
    {
        public PruneAggressive(GitWorkingDirectory gitworkingdirectory) : base(gitworkingdirectory)
        {
            var reflog1 = RunGit("reflog1", "reflog expire --expire-unreachable=now --all");
            reflog1.WaitFor();

            var reflog2 = RunGit("reflog2", "reflog expire --expire=now --all");
            reflog2.WaitFor();

            RunGit("prune", "gc --prune=now --aggressive");
        }

        protected override void RunGitDone()
        {
            var prune = GetGitExecutor("prune");
            //result can't be determined
        }
    }
}
