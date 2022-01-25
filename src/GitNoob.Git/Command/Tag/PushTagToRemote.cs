using System;

namespace GitNoob.Git.Command.Tag
{
    public class PushTagToRemote : Command
    {
        public bool? result { get; private set; }
        public string output { get; private set; }

        public PushTagToRemote(GitWorkingDirectory gitworkingdirectory, string remotename, string tagname) : base(gitworkingdirectory)
        {
            result = null;
            output = null;

            RunGit("push", "push --quiet \"" + remotename + "\" \"refs/tags/" + tagname + "\"");
        }

        protected override void RunGitDone()
        {
            var executor = GetGitExecutor("push");

            output = executor.Output.Trim();
            if (!String.IsNullOrWhiteSpace(output)) output += Environment.NewLine + Environment.NewLine;
            output += executor.Error.Trim();

            result = String.IsNullOrWhiteSpace(output);
        }
    }
}
