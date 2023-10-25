using GitNoob.Gui.Visualizer;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Step
{
    public class AskCherryPickCommitId : Step
    {
        public AskCherryPickCommitId() : base() { }

        protected override bool run()
        {
            //not really a failure, but a solution to input commit-id
            var message = new VisualizerMessageWithLinks();
            FailureRemedy = new Remedy.InputCherryPickCommitId(this, message, 
                (commitId) => {
                var step = new CherryPick(commitId);
                StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });
            });
            return false;
        }
    }
}
