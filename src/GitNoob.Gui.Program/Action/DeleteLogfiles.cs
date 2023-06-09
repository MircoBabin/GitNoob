using GitNoob.Utils;
using System.Collections.Generic;
using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public class DeleteLogfiles : Action
    {
        public DeleteLogfiles(ProgramWorkingDirectory Config) : base(Config) { }

        public override bool isStartable()
        {
            return (StartExploreLogfiles.GetPaths(config) != null);
        }

        public override Icon icon()
        {
            return Resources.getIcon("delete logfiles");
        }

        public override void execute()
        {
            if (!isStartable()) return;

            if (config.Visualizer.isFrontendLocked()) return;

            var steps = new List<StepsExecutor.IExecutableByStepsExecutor>();
            foreach(var path in StartExploreLogfiles.GetPaths(config))
            {
                steps.Add(new Step.DeleteLogfiles(path));
            }

            var executor = new StepsExecutor.StepsExecutor(config, steps);
            executor.execute();
        }
    }
}
