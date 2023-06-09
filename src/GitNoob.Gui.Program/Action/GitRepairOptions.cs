using GitNoob.Utils;
using System.Collections.Generic;
using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public class GitRepairOptions : Action
    {
        public GitRepairOptions(ProgramWorkingDirectory Config) : base(Config) { }

        public override bool isStartable()
        {
            return true;
        }

        public override Icon icon()
        {
            return Resources.getIcon("git repair");
        }

        public override void execute()
        {
            if (config.Visualizer.isFrontendLocked()) return;

            var executor = new StepsExecutor.StepsExecutor(config, new List<StepsExecutor.IExecutableByStepsExecutor>()
            {
                new Step.AskGitRepairOption(),
            });
            executor.execute();
        }
    }
}
