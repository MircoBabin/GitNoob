namespace GitNoob.Git.Result
{
    public class SetRemoteForBranchResult
    {
        public bool ErrorSettingRemoteUrl { get; set; }
        public bool ErrorSettingRemoteForBranch { get; set; }

        public bool RemoteSet { get; set; }

        public SetRemoteForBranchResult()
        {
            ErrorSettingRemoteUrl = false;
            ErrorSettingRemoteForBranch = false;

            RemoteSet = false;
        }
    }
}
