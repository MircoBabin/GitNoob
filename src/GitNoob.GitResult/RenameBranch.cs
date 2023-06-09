namespace GitNoob.GitResult
{
    public class RenameBranchResult
    {
        public bool Renamed { get; set; }

        public bool ErrorRebaseInProgress { get; set; }
        public bool ErrorMergeInProgress { get; set; }
        public bool ErrorRenaming { get; set; }

        public RenameBranchResult()
        {
            Renamed = false;
            ErrorRebaseInProgress = false;
            ErrorMergeInProgress = false;
            ErrorRenaming = false;
        }
    }
}
