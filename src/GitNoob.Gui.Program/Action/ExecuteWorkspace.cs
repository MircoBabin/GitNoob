using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace GitNoob.Gui.Program.Action
{
    public class ExecuteWorkspace : Action, IAction
    {
        public ExecuteWorkspace(StepsExecutor.StepConfig Config) : base(Config) { }

        private string GetFile()
        {
            if (stepConfig.Config.ProjectWorkingDirectory.Editor.WorkspaceFilename.isEmpty()) return null;

            return stepConfig.Config.ProjectWorkingDirectory.Editor.WorkspaceFilename.ToString();
        }

        public bool isStartable()
        {
            var file = GetFile();
            if (file == null) return false;
            if (!File.Exists(file)) return false;

            return true;
        }

        public Icon icon()
        {
            return Utils.ImageUtils.LoadIconForFile(stepConfig.Config.ProjectWorkingDirectory.Editor.WorkspaceFilename.ToString());
        }

        public void execute()
        {

            if (stepConfig.Visualizer.isFrontendLocked()) return;

            string filename = GetFile();
            bool asAdministrator = stepConfig.Config.ProjectWorkingDirectory.Editor.WorkspaceRunAsAdministrator.Value;

            var executor = new StepsExecutor.StepsExecutor(stepConfig, new List<StepsExecutor.IExecutableByStepsExecutor>()
            {
                new Step.StartWorkspace(filename, asAdministrator),
            });
            executor.execute();
        }
    }
}
