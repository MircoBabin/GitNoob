namespace GitNoob.GitResult
{
    public class StageAllChangesOnCurrentBranchResult
    {
        public bool ErrorDetachedHead { get; set; }
        public bool ErrorRebaseInProgress { get; set; }
        public bool ErrorMergeInProgress { get; set; }

        public bool Staged { get; set; }

        public StageAllChangesOnCurrentBranchResult()
        {
            ErrorDetachedHead = false;
            ErrorRebaseInProgress = false;
            ErrorMergeInProgress = false;

            Staged = false;
        }
    }
}

