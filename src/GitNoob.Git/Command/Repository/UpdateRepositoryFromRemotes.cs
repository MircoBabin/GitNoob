using GitNoob.Git.Command.Branch;
using System;

namespace GitNoob.Git.Command.Repository
{
    public class UpdateRepositoryFromRemotes : Command
    {
        //public bool? result { get; private set; }
        public string output { get; private set; }

        public UpdateRepositoryFromRemotes(GitWorkingDirectory gitworkingdirectory) : base(gitworkingdirectory)
        {
            //result = null;
            output = null;

            RunGit("fetch", "fetch --all --tags --prune --quiet").WaitFor();

            var branches = new ListBranches(gitworkingdirectory, false);
            branches.WaitFor();

            foreach(var branch in branches.result)
            {
                if (branch.Type == GitBranch.BranchType.LocalTrackingRemoteBranch)
                {
                    var ff = new FastForwardBranchToRemote(gitworkingdirectory, branch);
                    ff.WaitFor();
                }
            }
        }

        protected override void RunGitDone()
        {
            var executor = GetGitExecutor("fetch");

            output = executor.Output.Trim();
            if (!String.IsNullOrWhiteSpace(output)) output += Environment.NewLine + Environment.NewLine;
            output += executor.Error.Trim();

            //result can't be determined
        }
    }
}
