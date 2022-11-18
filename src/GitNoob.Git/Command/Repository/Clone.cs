﻿namespace GitNoob.Git.Command.Repository
{
    public class Clone : Command
    {
        //public string result { get; private set; }

        public Clone(GitWorkingDirectory gitworkingdirectory, string checkoutBranchName) : base(gitworkingdirectory)
        {
            //result = null;

            if (string.IsNullOrWhiteSpace(_gitworkingdirectory.RemoteUrl))
                throw new System.Exception("RemoteUrl is empty");

            RunGit("clone", "clone " + (!string.IsNullOrWhiteSpace(checkoutBranchName) ? "--branch \"" + checkoutBranchName + "\" " : "--no-checkout ") +
                "\"" + _gitworkingdirectory.RemoteUrl + "\" \"" + _gitworkingdirectory.WorkingPath + "\"");
        }

        protected override void RunGitDone()
        {
            var clone = GetGitExecutor("clone");
            //result can't be determined
        }
    }
}
