using GitNoob.GitResult;
using GitNoob.Utils;
using System;

namespace GitNoob.Git
{
    public partial class GitWorkingDirectory
    {
        public MergeResult MergeFastForwardOnlyCurrentBranchIntoMainBranch()
        {
            var result = new MergeResult();
            if (GitDisaster.Check(this, result))
                return result;
            var currentbranch = result.GitDisaster_CurrentBranchShortName;

            var lastcommit = new Command.Branch.GetLastCommitOfBranch(this, currentbranch);
            lastcommit.WaitFor();
            if (lastcommit.commitmessage == null)
            {
                result.ErrorRetrievingLastCommit = true;
                return result;
            }
            bool lastCommitIsTemporary = lastcommit.commitmessage.StartsWith(TemporaryCommitMessage);

            Command.Branch.MergeFastForwardOnly merge;
            if (!lastCommitIsTemporary)
            {
                merge = new Command.Branch.MergeFastForwardOnly(this, currentbranch, MainBranch);
                merge.WaitFor();
                //Because of fast-forward-only there can be no conflicts.
            }
            else
            {
                //create branch on previous commit (last commit is a temporary commit)
                string randomsha1 = GitUtils.GenerateRandomSha1();
                string tempbranch = "gitnoob-temp-" + randomsha1;

                var newbranch = new Command.Branch.CreateBranch(this, tempbranch, currentbranch + "^", false);
                newbranch.WaitFor();
                try
                {
                    merge = new Command.Branch.MergeFastForwardOnly(this, tempbranch, MainBranch);
                    merge.WaitFor();
                    //Because of fast-forward-only there can be no conflicts.
                }
                finally
                {
                    var delnewbranch = new Command.Branch.DeleteBranch(this, tempbranch, true);
                    delnewbranch.WaitFor();
                }
            }

            //restore current branch
            var change = new Command.Branch.ChangeBranchTo(this, currentbranch);
            change.WaitFor();

            result.Merged = merge.result == true;
            return result;
        }

        public MergeResult MergeContinue()
        {
            var result = new MergeResult();
            if (GitDisaster.Check(this, result, new GitDisasterAllowed() { Allow_MergeInProgress = true }))
                return result;

            if (result.GitDisaster_MergeInProgress != true)
            {
                result.ErrorNotMerging = true;
                return result;
            }

            var merge = new Command.Branch.MergeContinue(this);
            merge.WaitFor();

            var conflicts = new Command.WorkingTree.HasConflicts(this);
            var merging = new Command.WorkingTree.IsMergeActive(this);
            conflicts.WaitFor();
            merging.WaitFor();

            if (conflicts.result != false) result.ErrorConflicts = true;
            if (merging.result == false) result.Merged = true;

            return result;
        }

        public MergeResult MergeAbort(string ChangeBranchTo)
        {
            var result = new MergeResult();
            if (GitDisaster.Check(this, result, new GitDisasterAllowed() { 
                Allow_MergeInProgress = true,
                Allow_WorkingTreeChanges = true,
                Allow_StagedUncommittedFiles = true,
            }))
                return result;

            if (result.GitDisaster_MergeInProgress != true)
            {
                result.ErrorNotMerging = true;
                return result;
            }

            var merge = new Command.Branch.MergeAbort(this);
            merge.WaitFor();

            var merging = new Command.WorkingTree.IsMergeActive(this);
            merging.WaitFor();

            if (merging.result != false)
            {
                result.Aborted = false;
                return result;
            }
                
            result.Aborted = true;

            if (!String.IsNullOrWhiteSpace(ChangeBranchTo))
            {
                var change = new Command.Branch.ChangeBranchTo(this, ChangeBranchTo);
                change.WaitFor();
            }

            return result;
        }
    }
}
