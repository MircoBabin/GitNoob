using System.Collections.Generic;
using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public class ExecuteDeleteAllChanges : Action, IAction
    {
        public ExecuteDeleteAllChanges(StepsExecutor.StepConfig Config) : base(Config) { }

        public Icon icon()
        {
            return Utils.Resources.getIcon("delete all changes");
        }

        public void execute()
        {
            if (stepConfig.Visualizer.isFrontendLocked()) return;

            var executor = new StepsExecutor.StepsExecutor(stepConfig, new List<StepsExecutor.IExecutableByStepsExecutor>()
            {
                new Step.AskDeleteAllChanges()
            });
            executor.execute();
        }
    }
}
