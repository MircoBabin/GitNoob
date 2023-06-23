using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action
{
    public class StartGitkForCurrentBranch : StartGitk
    {
        public StartGitkForCurrentBranch(ProgramWorkingDirectory Config) : base(Config) { }

        public override void execute()
        {
            var result = config.Git.RetrieveMainBranch();

            executeGitk(new List<string>() {
                "HEAD",
                config.ProjectWorkingDirectory.Git.MainBranch,
                (result != null ? result.RemoteBranchFullName : null)
            });
        }
    }
}
