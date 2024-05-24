using GitNoob.GitResult;

namespace GitNoob.Git
{
    public partial class GitWorkingDirectory
    {
        public RebaseResult RebaseCurrentBranchOntoMainBranch(string createUndeleteTagMessage)
        {
            var result = new RebaseResult();
            if (GitDisaster.Check(this, result))
                return result;
            var currentbranch = result.GitDisaster_CurrentBranchShortName;

            if (!string.IsNullOrWhiteSpace(createUndeleteTagMessage))
            {
                if (!CreateDeletedBranchUndoTag(currentbranch, MainBranch, createUndeleteTagMessage))
                {
                    result.ErrorCreatingSafetyTag = true;
                    return result;
                }
            }

            var rebase = new Command.Branch.RebaseCurrentBranch(this, MainBranch);
            rebase.WaitFor();

            var conflicts = new Command.WorkingTree.HasConflicts(this);
            var rebasing = new Command.WorkingTree.IsRebaseActive(this);
            conflicts.WaitFor();
            rebasing.WaitFor();

            if (conflicts.result != false) result.ErrorConflicts = true;
            if (rebasing.result == false) result.Rebased = true;

            return result;
        }

        public RebaseResult RebaseContinue()
        {
            var result = new RebaseResult();
            if (GitDisaster.Check(this, result, new GitDisasterAllowed() { Allow_RebaseInProgress = true }))
                return result;

            if (result.GitDisaster_RebaseInProgress != true)
            {
                result.ErrorNotRebasing = true;
                return result;
            }

            var rebase = new Command.Branch.RebaseContinue(this);
            rebase.WaitFor();

            var conflicts = new Command.WorkingTree.HasConflicts(this);
            var rebasing = new Command.WorkingTree.IsRebaseActive(this);
            conflicts.WaitFor();
            rebasing.WaitFor();

            if (conflicts.result != false) result.ErrorConflicts = true;
            if (rebasing.result == false) result.Rebased = true;

            return result;
        }

        public RebaseResult RebaseAbort()
        {
            var result = new RebaseResult();
            if (GitDisaster.Check(this, result, new GitDisasterAllowed() { 
                Allow_DetachedHead = true,
                Allow_RebaseInProgress = true,
                Allow_WorkingTreeChanges = true,
                Allow_StagedUncommittedFiles = true,
            }))
                return result;

            if (result.GitDisaster_RebaseInProgress != true)
            {
                result.ErrorNotRebasing = true;
                return result;
            }

            var rebase = new Command.Branch.RebaseAbort(this);
            rebase.WaitFor();

            var rebasing = new Command.WorkingTree.IsRebaseActive(this);
            rebasing.WaitFor();

            if (rebasing.result == false) result.Aborted = true;

            return result;
        }
    }
}
