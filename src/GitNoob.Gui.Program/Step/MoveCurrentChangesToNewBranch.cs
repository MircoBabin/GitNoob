using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class MoveCurrentChangesToNewBranch : Step
    {
        private string _newBranch;
        public MoveCurrentChangesToNewBranch(string NewBranch) : base()
        {
            _newBranch = NewBranch;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - moving workingtree changes and staged uncommitted files to new branch";

            var result = StepsExecutor.Config.Git.MoveCurrentChangesToNewBranch(_newBranch);

            var message = new VisualizerMessageWithLinks("Move changes is not possible.");

            if (result.IsGitDisasterHappening != false)
            {
                FailureRemedy = new Remedy.MessageGitDisaster(this, message, result);
                return false;
            }

            StepsExecutor.CurrentBranchChangedTo(_newBranch);

            if (result.ErrorBranchAlreadyExists)
            {
                FailureRemedy = new Remedy.MessageBranchAlreadyExists(this, message, _newBranch);
                return false;
            }

            if (result.Created)
            {
                StepsExecutor.CurrentBranchChangedTo(result.CurrentBranch);
            }
            else
            {
                StepsExecutor.CurrentBranchChangedTo(null);
            }

            return true;
        }
    }
}
