namespace GitNoob.Gui.Program.Action
{
    public class StartDosPromptAsUser : StartDosPrompt
    {
        public StartDosPromptAsUser(ProgramWorkingDirectory Config) : base(Config) { }

        public override void execute()
        {
            executeDosPrompt(false);
        }
    }
}
