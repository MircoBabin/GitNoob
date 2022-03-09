using System.Collections.Generic;

namespace GitNoob.Git.Result
{
    public class RemotesResult
    {
        public string CurrentBranch { get; set; }
        public List<GitRemote> Remotes { get; set; }

        public RemotesResult()
        {
            Remotes = new List<GitRemote>();
        }
    }
}
