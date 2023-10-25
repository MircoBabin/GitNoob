using System;

namespace GitNoob.GitResult
{
    public class MergeResult : BaseGitDisasterResult
    {
        public bool Merged { get; set; }
        public bool Aborted { get; set; }

        public bool ErrorRetrievingLastCommit { get; set; }
        public bool ErrorConflicts { get; set; }

        public bool ErrorNotMerging { get; set; }

        public MergeResult() : base()
        {
            Merged = false;
            Aborted = false;

            ErrorRetrievingLastCommit = false;
            ErrorConflicts = false;

            ErrorNotMerging = false;
        }
    }
}

