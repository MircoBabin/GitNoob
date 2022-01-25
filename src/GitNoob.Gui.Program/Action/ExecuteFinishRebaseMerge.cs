using System.Collections.Generic;
using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public class ExecuteFinishRebaseMerge : Action, IAction
    {
        public ExecuteFinishRebaseMerge(StepsExecutor.StepConfig Config) : base(Config) { }

        public Icon icon()
        {
            return null;
        }

        public void execute()
        {
            if (stepConfig.Visualizer.isFrontendLocked()) return;

            var executor = new StepsExecutor.StepsExecutor(stepConfig, new List<StepsExecutor.IExecutableByStepsExecutor>()
            {
                new Step.FinishRebaseMerge()
            });
            executor.execute();
        }
    }
}
