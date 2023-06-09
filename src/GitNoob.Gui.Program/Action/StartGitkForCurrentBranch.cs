using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action
{
    public class StartGitkForCurrentBranch : StartGitk
    {
        public StartGitkForCurrentBranch(ProgramWorkingDirectory Config) : base(Config) { }

        public override void execute()
        {
            executeGitk(new List<string>() { "HEAD" });
        }
    }
}
