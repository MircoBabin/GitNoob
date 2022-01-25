using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace GitNoob.Gui.Program.Action
{
    public class ExecuteOpenConfigfiles : Action, IAction
    {
        public ExecuteOpenConfigfiles(StepsExecutor.StepConfig Config) : base(Config) { }

        public IEnumerable<string> GetFiles()
        {
            if (stepConfig.Config.ProjectWorkingDirectory.ProjectType == null) return null;

            var files = stepConfig.Config.ProjectWorkingDirectory.ProjectType.GetConfigurationFiles();
            if (files == null) return null;

            var result = new List<string>();
            foreach(var file in files)
            {
                result.Add(Path.Combine(stepConfig.Config.ProjectWorkingDirectory.Path, file));
            }

            if (result.Count == 0) return null;

            return result;
        }

        public bool isStartable()
        {
            return (GetFiles() != null);
        }

        public Icon icon()
        {
            return Utils.Resources.getIcon("open configfiles");
        }

        public void execute()
        {
            if (!isStartable()) return;

            if (stepConfig.Visualizer.isFrontendLocked()) return;

            var executor = new StepsExecutor.StepsExecutor(stepConfig, new List<StepsExecutor.IExecutableByStepsExecutor>()
            {
                new Step.EditFiles(GetFiles()),
            });
            executor.execute();
        }
    }
}
