namespace GitNoob.Gui.Program.Action.Step
{
    public class SetCommitter : Step
    {
        private string _name;
        private string _email;
        public SetCommitter(string name, string email) : base()
        {
            _name = name;
            _email = email;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - setting commit name to " + _name + " <" + _email + ">";

            var result = StepsExecutor.Config.Git.ChangeCommitter(_name, _email);

            var message = new VisualizerMessageWithLinks("Set commit name to " + _name + " <" + _email + "> failed.");

            if (result.ErrorChangingName || result.ErrorChangingEmail)
            {
                FailureRemedy = new Remedy.MessageSetCommitterFailed(this, message, result.ErrorChangingName, result.ErrorChangingEmail);
                return false;
            }

            if (!result.Changed)
            {
                FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                return false;
            }

            return true;
        }
    }
}

