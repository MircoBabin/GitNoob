using GitNoob.GitResult;

namespace GitNoob.Git
{
    public partial class GitWorkingDirectory
    {
        public GitReferenceLogResult RetrieveGitReferenceLog()
        {
            var result = new GitReferenceLogResult();

            var command = new Command.Repository.ListReflog(this);
            command.WaitFor();
            if (command.result != null)
            {
                result.GitReferenceLog = command.result;
            }

            result.Sort();

            return result;
        }

        public CreateNewBranchResult CreateBranchOnGitReferenceLog(GitReflog reflog, string branchname, bool checkoutNewBranch)
        {
            var result = CreateNewBranch(branchname, reflog.CommitId, checkoutNewBranch);

            return result;
        }
    }
}
