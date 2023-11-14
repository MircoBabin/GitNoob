using GitNoob.GitResult;

namespace GitNoob.Git
{
    public partial class GitWorkingDirectory
    {
        public CherryPickResult CherryPick(string commitId)
        {
            var result = new CherryPickResult();
            if (GitDisaster.Check(this, result))
                return result;

            var lastcommit0 = new Command.Branch.GetLastCommitOfBranch(this, result.GitDisaster_CurrentBranchShortName);
            lastcommit0.WaitFor();

            var command = new Command.Branch.CherryPickOneCommit(this, commitId);
            command.WaitFor();

            var conflicts = new Command.WorkingTree.HasConflicts(this);
            var cherrypicking = new Command.WorkingTree.IsCherryPickActive(this);
            var lastcommit1 = new Command.Branch.GetLastCommitOfBranch(this, result.GitDisaster_CurrentBranchShortName);
            conflicts.WaitFor();
            cherrypicking.WaitFor();
            lastcommit1.WaitFor();

            if (conflicts.result != false) result.ErrorConflicts = true;
            if (lastcommit0.commitid == lastcommit1.commitid) result.NothingCherryPicked = true;
            if (cherrypicking.result == false && lastcommit0.commitid != lastcommit1.commitid) result.CherryPicked = true;

            return result;
        }

        public CherryPickResult CherryPickContinue()
        {
            var result = new CherryPickResult();
            if (GitDisaster.Check(this, result, new GitDisasterAllowed() { Allow_CherryPickInProgress = true }))
                return result;

            if (result.GitDisaster_CherryPickInProgress != true)
            {
                result.ErrorNotCherryPicking = true;
                return result;
            }

            var command = new Command.Branch.CherryPickContinue(this);
            command.WaitFor();

            var conflicts = new Command.WorkingTree.HasConflicts(this);
            var cherrypicking = new Command.WorkingTree.IsCherryPickActive(this);
            conflicts.WaitFor();
            cherrypicking.WaitFor();

            if (conflicts.result != false) result.ErrorConflicts = true;
            if (cherrypicking.result == false) result.CherryPicked = true;

            return result;
        }

        public CherryPickResult CherryPickAbort()
        {
            var result = new CherryPickResult();
            if (GitDisaster.Check(this, result, new GitDisasterAllowed() { 
                Allow_CherryPickInProgress = true,
                Allow_WorkingTreeChanges = true,
                Allow_StagedUncommittedFiles = true,
            }))
                return result;

            if (result.GitDisaster_CherryPickInProgress != true)
            {
                result.ErrorNotCherryPicking = true;
                return result;
            }

            var command = new Command.Branch.CherryPickAbort(this);
            command.WaitFor();

            var cherrypicking = new Command.WorkingTree.IsCherryPickActive(this);
            cherrypicking.WaitFor();

            if (cherrypicking.result == false) result.Aborted = true;

            return result;
        }
    }
}
