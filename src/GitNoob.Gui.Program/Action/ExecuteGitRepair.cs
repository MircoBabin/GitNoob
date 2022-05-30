using System.Collections.Generic;
using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public class ExecuteGitRepair : Action, IAction
    {
        public ExecuteGitRepair(StepsExecutor.StepConfig Config) : base(Config) { }

        public Icon icon()
        {
            return Utils.Resources.getIcon("git repair");
        }

        public void execute()
        {

            if (stepConfig.Visualizer.isFrontendLocked()) return;

            var executor = new StepsExecutor.StepsExecutor(stepConfig, new List<StepsExecutor.IExecutableByStepsExecutor>()
            {
                new Step.AskGitRepairOption(),
            });
            executor.execute();
        }
    }
}
