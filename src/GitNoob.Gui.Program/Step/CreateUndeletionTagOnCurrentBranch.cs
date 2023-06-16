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

                if (result.ErrorRebaseInProgress || result.ErrorMergeInProgress)
                {
                    FailureRemedy = new Remedy.MessageRebasingOrMerging(this, message, result.ErrorRebaseInProgress, result.ErrorMergeInProgress);
                    return false;
                }

                if (result.ErrorDetachedHead)
                {
                    FailureRemedy = new Remedy.MessageDetachedHead(this, message);
                    return false;
                }

                if (result.ErrorWorkingTreeChanges || result.ErrorStagedUncommittedFiles)
                {
                    FailureRemedy = new Remedy.MessageChanges(this, message, result.ErrorWorkingTreeChanges, result.ErrorStagedUncommittedFiles);
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
