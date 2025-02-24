namespace GitNoob.Git.Command.WorkingTree
{
    public class HasConflicts : Command
    {
        public bool? result { get; private set; }

        public HasConflicts(GitWorkingDirectory gitworkingdirectory) : base(gitworkingdirectory)
        {
            result = null;

            RunGit("unmerged", new string[] { "ls-files", "--exclude-standard", "--unmerged" });
        }

        protected override void RunGitDone()
        {
            {
                var cmd = GetGitExecutor("unmerged");
                result = (cmd.Output.Trim().Length > 0);
            }
        }
    }
}
