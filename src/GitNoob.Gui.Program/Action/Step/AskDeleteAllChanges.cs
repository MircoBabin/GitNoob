namespace GitNoob.Gui.Program.Action.Step
{
    public class AskDeleteAllChanges : Step
    {
        public AskDeleteAllChanges() : base() { }

        protected override bool run()
        {
            //not really a failure, but a solution to ask for confirmation
            var message = new MessageWithLinks();

            FailureRemedy = new Remedy.InputConfirmDeleteAllChanges(this, message);
            return false;
        }
    }
}
