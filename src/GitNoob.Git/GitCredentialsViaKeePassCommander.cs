using System;

namespace GitNoob.Git
{
    public static class GitCredentialsViaKeePassCommander
    {
        public static bool AreCredentialsAvailable(GitWorkingDirectory gitworkingdirectory)
        {
            if (String.IsNullOrWhiteSpace(gitworkingdirectory.RemoteUrl)) return true;

            if (!gitworkingdirectory.RemoteUrl.StartsWith("http://") && 
                !gitworkingdirectory.RemoteUrl.StartsWith("https://")) return true;

            var credentialhelper = new Command.Config.GetCredentialHelper(gitworkingdirectory);
            credentialhelper.WaitFor();
            if (!credentialhelper.result.Contains("git-credential-keepasscommand.exe")) return true;

            return credentialhelper.TryCommandGet();
        }
    }
}
