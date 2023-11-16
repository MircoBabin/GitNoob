namespace GitNoob.GitResult
{
    public class SetRemoteForBranchResult : BaseGitDisasterResult
    {
        public bool ErrorSettingRemoteUrl { get; set; }
        public bool ErrorSettingRemoteForBranch { get; set; }

        public bool RemoteSet { get; set; }

        public SetRemoteForBranchResult() : base()
        {
            ErrorSettingRemoteUrl = false;
            ErrorSettingRemoteForBranch = false;

            RemoteSet = false;
        }
    }
}
