using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class CreateUndeletionTagOnCurrentBranch : Step
    {
        private string _message;

        public CreateUndeletionTagOnCurrentBranch(string Message) : base()
        {
            _message = Message;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - creating an undelete entry";

            {
                var result = StepsExecutor.Config.Git.CreateUndeletionTagOnCurrentBranch(_message);

                var message = new VisualizerMessageWithLinks("Creating an undelete entry failed.");

                if (result.IsGitDisasterHappening != false)
                {
                    FailureRemedy = new Remedy.MessageGitDisaster(this, message, result);
                    return false;
                }

                if (!result.Created)
                {
                    FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                    return false;
                }
            }

            return true;
        }
    }
}
