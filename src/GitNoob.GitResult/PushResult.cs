using System;

namespace GitNoob.GitResult
{
    public class PushResult
    {
        public bool Pushed { get; set; }
        public string PushOutput { get; set; }

        public bool ErrorKeePassNotStarted { get; set; }
        public bool ErrorRemoteNotReachable { get; set; }

        public bool ErrorStagedUncommittedFiles { get; set; }
        public bool ErrorWorkingTreeChanges { get; set; }
        public bool ErrorRebaseInProgress { get; set; }
        public bool ErrorMergeInProgress { get; set; }

        public bool ErrorStillUnpushedCommitsOnMainBranch { get; set; }
        public bool ErrorConflicts { get; set; }

        public PushResult()
        {
            Pushed = false;
            PushOutput = String.Empty;

            ErrorKeePassNotStarted = false;
            ErrorRemoteNotReachable = false;

            ErrorStagedUncommittedFiles = false;
            ErrorWorkingTreeChanges = false;
            ErrorRebaseInProgress = false;
            ErrorMergeInProgress = false;

            ErrorStillUnpushedCommitsOnMainBranch = false;
            ErrorConflicts = false;
        }
    }
}
