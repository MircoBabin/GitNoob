namespace GitNoob.Gui.Program.Action.Step
{
    public class SetRemoteForBranch : Step
    {
        private string _branchName;
        private string _remoteName;
        private string _remoteUrl;

        public SetRemoteForBranch(string BranchName, string RemoteName, string RemoteUrl = null) : base()
        {
            _branchName = BranchName;
            _remoteName = RemoteName;
            _remoteUrl = RemoteUrl;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - setting remote for \"" + _branchName + "\"";

            var message = new MessageWithLinks("Setting remote for \"" + _branchName + "\" failed.");

            var result = StepsExecutor.Config.Git.SetRemoteForBranch(_branchName, _remoteName, _remoteUrl);
            if (result.ErrorSettingRemoteUrl)
            {
                FailureRemedy = new Remedy.MessageSetRemoteUrlFailed(this, message, _remoteName, _remoteUrl);
                return false;
            }

            if (result.ErrorSettingRemoteForBranch)
            {
                FailureRemedy = new Remedy.MessageSetRemoteForBranchFailed(this, message, _branchName, _remoteName);
                return false;
            }

            if (!result.RemoteSet)
            {
                FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                return false;
            }

            return true;
        }
    }
}
