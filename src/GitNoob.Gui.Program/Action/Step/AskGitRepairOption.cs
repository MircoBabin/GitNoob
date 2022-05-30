namespace GitNoob.Gui.Program.Action.Step
{
    public class AskGitRepairOption : Step
    {
        public AskGitRepairOption() : base() { }

        protected override bool run()
        {
            //not really a failure, but a solution to ask for confirmation
            var message = new MessageWithLinks();

            FailureRemedy = new Remedy.SelectGitRepairOption(this, message);
            return false;
        }
    }
}
