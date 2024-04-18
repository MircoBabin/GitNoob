using System;

namespace GitNoob.GitResult
{
    public class PushResult : BaseGitDisasterResult
    {
        public bool NothingToPush_HasNoGitNoobRemoteUrl { get; set; }
        public bool Pushed { get; set; }
        public string PushOutput { get; set; }

        public bool ErrorKeePassNotStarted { get; set; }
        public bool ErrorRemoteNotReachable { get; set; }

        public bool ErrorStillUnpushedCommitsOnMainBranch { get; set; }
        public bool ErrorConflicts { get; set; }

        public PushResult() : base()
        {
            NothingToPush_HasNoGitNoobRemoteUrl = false;
            Pushed = false;
            PushOutput = String.Empty;

            ErrorKeePassNotStarted = false;
            ErrorRemoteNotReachable = false;

            ErrorStillUnpushedCommitsOnMainBranch = false;
            ErrorConflicts = false;
        }
    }
}
