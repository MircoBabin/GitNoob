using System.Collections.Generic;

namespace GitNoob.Git.Result
{
    public class BranchesResult
    {
        public string CurrentBranch { get; set; }
        public List<string> Branches { get; set; }

        public BranchesResult()
        {
            CurrentBranch = string.Empty;
            Branches = new List<string>();
        }
    }
}
