using System.Collections.Generic;

namespace GitNoob.Git.Result
{
    public class DeletedBranchesResult
    {
        public List<GitDeletedBranch> DeletedBranches { get; set; }

        public DeletedBranchesResult()
        {
            DeletedBranches = new List<GitDeletedBranch>();
        }
    }
}
