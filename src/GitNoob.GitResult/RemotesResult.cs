using System.Collections.Generic;

namespace GitNoob.GitResult
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
