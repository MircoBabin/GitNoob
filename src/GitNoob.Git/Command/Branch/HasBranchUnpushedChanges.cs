namespace GitNoob.Git.Command.Branch
{
    public class HasBranchUnpushedChanges : Command
    {
        public enum TrackingRemoteBranch { Yes, NoAndNoLocalCommits, NoAndHasLocalCommits };
        public TrackingRemoteBranch? IsTrackingRemoteBranch { get; private set; }
        public GetRemoteBranch RemoteBranch;

        public bool? result { get; private set; }

        public HasBranchUnpushedChanges(GitWorkingDirectory gitworkingdirectory, string localBranch, string mainBranch) : base(gitworkingdirectory)
        {
            IsTrackingRemoteBranch = null;
            result = null;

            RemoteBranch = new GetRemoteBranch(gitworkingdirectory, localBranch);
            RemoteBranch.WaitFor();

            if (!string.IsNullOrWhiteSpace(RemoteBranch.result))
            {
                IsTrackingRemoteBranch = TrackingRemoteBranch.Yes;
                RunGit("base", "rev-list -n 1 \"" + RemoteBranch.result + ".." + localBranch + "\"");
            }
            else
            {
                RunGit("base", "rev-list -n 1 \"" + mainBranch + ".." + localBranch + "\"");
            }
        }

        protected override void RunGitDone()
        {
            if (result == null)
            {
                var executor = GetGitExecutor("base");
                result = !string.IsNullOrWhiteSpace(executor.Output.Trim());

                if (IsTrackingRemoteBranch != TrackingRemoteBranch.Yes)
                {
                    if (result == true)
                        IsTrackingRemoteBranch = TrackingRemoteBranch.NoAndHasLocalCommits;
                    else
                        IsTrackingRemoteBranch = TrackingRemoteBranch.NoAndNoLocalCommits;
                }
            }
        }
    }
}
