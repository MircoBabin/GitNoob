using System;

namespace GitNoob.Git.Command.Branch
{
    public class ChangeBranchTo : Command
    {
        public bool? result;

        public ChangeBranchTo(GitWorkingDirectory gitworkingdirectory, string localBranch) : base(gitworkingdirectory)
        {
            result = null;

            RunGit("checkout", new string[] { "checkout", "--quiet", localBranch });
        }

        protected override void RunGitDone()
        {
            var executor = GetGitExecutor("checkout");
            result = String.IsNullOrWhiteSpace(executor.Error.Trim());
        }
    }
}
