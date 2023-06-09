namespace GitNoob.Gui.Program.Action
{
    public class StartDosPromptAsAdministrator : StartDosPrompt
    {
        public StartDosPromptAsAdministrator(ProgramWorkingDirectory Config) : base(Config) { }

        public override void execute()
        {
            executeDosPrompt(true);
        }
    }
}
