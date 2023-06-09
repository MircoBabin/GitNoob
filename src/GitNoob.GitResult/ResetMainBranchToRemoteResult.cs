namespace GitNoob.GitResult
{
    public class ResetMainBranchToRemoteResult
    {
        public bool Reset { get; set; }

        public bool ErrorStagedUncommittedFiles { get; set; }
        public bool ErrorWorkingTreeChanges { get; set; }
        public bool ErrorRebaseInProgress { get; set; }
        public bool ErrorMergeInProgress { get; set; }

        public bool ErrorCurrentBranchIsMainBranch { get; set; }
        public bool ErrorCurrentBranchCommitUnequalsMainBranchCommit { get; set; }

        public ResetMainBranchToRemoteResult()
        {
            Reset = false;

            ErrorStagedUncommittedFiles = false;
            ErrorWorkingTreeChanges = false;
            ErrorRebaseInProgress = false;
            ErrorMergeInProgress = false;

            ErrorCurrentBranchIsMainBranch = false;
            ErrorCurrentBranchCommitUnequalsMainBranchCommit = false;
        }
    }
}
