using System;

namespace GitNoob.GitResult
{
    public class CherryPickResult : BaseGitDisasterResult
    {
        public bool CherryPicked { get; set; }
        public bool NothingCherryPicked { get; set; }
        public bool Aborted { get; set; }

        public bool ErrorConflicts { get; set; }

        public bool ErrorNotCherryPicking { get; set; }

        public CherryPickResult() : base()
        {
            CherryPicked = false;
            NothingCherryPicked = false;
            Aborted = false;

            ErrorConflicts = false;

            ErrorNotCherryPicking = false;
        }
    }
}
