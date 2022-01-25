using System.Collections.Generic;
using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public class ExecuteChangeBranch : Action, IAction
    {
        public ExecuteChangeBranch(StepsExecutor.StepConfig Config) : base(Config) { }

        public Icon icon()
        {
            return null;
        }

        public void execute()
        {
            if (stepConfig.Visualizer.isFrontendLocked()) return;

            var executor = new StepsExecutor.StepsExecutor(stepConfig, new List<StepsExecutor.IExecutableByStepsExecutor>()
            {
                new Step.EnsureStatus("Change branch is not possible.", false, Step.EnsureStatus.WorkingTreeChanges.FalseAndCanTemporaryCommit, false, false, null),
                new Step.AskChangeBranch()
            });
            executor.execute();
        }
    }
}
