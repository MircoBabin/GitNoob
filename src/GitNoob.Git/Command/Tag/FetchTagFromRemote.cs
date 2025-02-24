using System;

namespace GitNoob.Git.Command.Tag
{
    public class FetchTagFromRemote : Command
    {
        //public bool? result { get; private set; }
        public string output { get; private set; }

        public FetchTagFromRemote(GitWorkingDirectory gitworkingdirectory, string remotename, string tagname) : base(gitworkingdirectory)
        {
            //result = null;
            output = null;

            //Overwrite local tag if it exists
            RunGit("fetch", new string[] { "fetch", remotename, "+refs/tags/" + tagname + ":refs/tags/" + tagname });
        }

        protected override void RunGitDone()
        {
            var executor = GetGitExecutor("fetch");

            output = executor.Output.Trim();
            if (!String.IsNullOrWhiteSpace(output)) output += Environment.NewLine + Environment.NewLine;
            output += executor.Error.Trim();

            //result can not be determined
        }
    }
}
