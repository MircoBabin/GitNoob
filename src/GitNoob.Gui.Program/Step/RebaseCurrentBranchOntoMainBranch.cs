using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class RebaseCurrentBranchOntoMainBranch : Step
    {
        private string _createUndeleteTagMessage;
        public RebaseCurrentBranchOntoMainBranch(string createUndeleteTagMessage) : base()
        {
            _createUndeleteTagMessage = createUndeleteTagMessage;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - rebasing current branch onto main branch";

            var result = StepsExecutor.Config.Git.RebaseCurrentBranchOntoMainBranch(_createUndeleteTagMessage);

            var message = new VisualizerMessageWithLinks("Rebasing current branch onto main branch failed.");

            if (result.IsGitDisasterHappening != false)
            {
                FailureRemedy = new Remedy.MessageGitDisaster(this, message, result);
                return false;
            }

            if (result.ErrorConflicts)
            {
                FailureRemedy = new Remedy.ResolveRebaseConflicts(this, message, MainBranch, result.GitDisaster_CurrentBranchShortName);
                return false;
            }

            if (!result.Rebased)
            {
                FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                return false;
            }

            return true;
        }
    }
}
