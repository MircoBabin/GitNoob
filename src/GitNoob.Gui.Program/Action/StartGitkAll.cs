namespace GitNoob.Gui.Program.Action
{
    public class StartGitkAll : StartGitk
    {
        public StartGitkAll(ProgramWorkingDirectory Config) : base(Config) { }

        public override void execute()
        {
            executeGitk(null);
        }
    }
}

