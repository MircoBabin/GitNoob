using System;

namespace GitNoob.GitResult
{
    public class RebaseResult : BaseGitDisasterResult
    {
        public bool Rebased { get; set; }
        public bool Aborted { get; set; }

        public bool ErrorConflicts { get; set; }

        public bool ErrorCreatingSafetyTag { get; set; }
        public bool ErrorNotRebasing { get; set; }

        public RebaseResult() : base()
        {
            Rebased = false;
            Aborted = false;

            ErrorConflicts = false;

            ErrorCreatingSafetyTag = false;
            ErrorNotRebasing = false;
        }
    }
}
