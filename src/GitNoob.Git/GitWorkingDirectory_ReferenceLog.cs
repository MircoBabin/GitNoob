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

        public ChangeCurrentBranchResult ChangeCurrentBranchTo(string branchname)
        {
            //explicitly no check for detached head.
            //to allow for resolving detached head state by checking out a branch.
            var changes = new Command.WorkingTree.HasChanges(this);
            var rebasing = new Command.WorkingTree.IsRebaseActive(this);
            var merging = new Command.WorkingTree.IsMergeActive(this);

            changes.WaitFor();
            rebasing.WaitFor();
            merging.WaitFor();

            if (changes.stagedUncommittedFiles != false || changes.workingtreeChanges != false ||
                rebasing.result != false || merging.result != false)
            {
                return new ChangeCurrentBranchResult()
                {
                    ErrorStagedUncommittedFiles = (changes.stagedUncommittedFiles != false),
                    ErrorWorkingTreeChanges = (changes.workingtreeChanges != false),
                    ErrorRebaseInProgress = (rebasing.result != false),
                    ErrorMergeInProgress = (merging.result != false),
                };
            }

            if (branchname.Contains("/"))
            {
                branchname = branchname.Substring(branchname.LastIndexOf("/") + 1);
            }

            var command = new Command.Branch.ChangeBranchTo(this, branchname);
            command.WaitFor();

            var currentbranch = new Command.Branch.GetCurrentBranch(this);
            currentbranch.WaitFor();

            if (currentbranch.shortname != branchname)
            {
                return new ChangeCurrentBranchResult()
                {
                    ErrorChanging = true,
                };
            }

            return new ChangeCurrentBranchResult()
            {
                Changed = true,
                CurrentBranch = currentbranch.shortname,
            };
        }
    }
}
