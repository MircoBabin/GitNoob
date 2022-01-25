namespace GitNoob.Git.Command.Stash
{
    public class StashWorkingTreeChanges : Command
    {
        public bool? result { get; private set; }

        private string stashName;

        public StashWorkingTreeChanges(GitWorkingDirectory gitworkingdirectory, string stashName) : base(gitworkingdirectory)
        {
            result = null;
            this.stashName = stashName;

            //Don't use --all, because popping will result in errors like 
            //- storage/framework/views/... already exists
            //- storage/logs/... already exists
            //- .env already exists
            RunGit("stash", "stash push --include-untracked --message \"" + stashName + "\"");
        }

        protected override void RunGitDone()
        {
            //Saved working directory and index state On wip: <stashName>
            var executor = GetGitExecutor("stash");
            result = executor.Output.Trim().Contains(stashName);
        }
    }
}
