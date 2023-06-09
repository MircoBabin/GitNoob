using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace GitNoob.Gui.Program.Action
{
    public class StartWorkspace : Action
    {
        public StartWorkspace(ProgramWorkingDirectory Config) : base(Config) { }

        private string GetFile()
        {
            if (config.ProjectWorkingDirectory.Editor.WorkspaceFilename.isEmpty()) return null;

            return config.ProjectWorkingDirectory.Editor.WorkspaceFilename.ToString();
        }

        public override bool isStartable()
        {
            var file = GetFile();
            if (file == null) return false;
            if (!File.Exists(file)) return false;

            return true;
        }

        public override Icon icon()
        {
            return Utils.ImageUtils.LoadIconForFile(config.ProjectWorkingDirectory.Editor.WorkspaceFilename.ToString());
        }

        public override void execute()
        {
            if (config.Visualizer.isFrontendLocked()) return;

            string filename = GetFile();
            bool asAdministrator = config.ProjectWorkingDirectory.Editor.WorkspaceRunAsAdministrator.Value;

            var executor = new StepsExecutor.StepsExecutor(config, new List<StepsExecutor.IExecutableByStepsExecutor>()
            {
                new Step.StartWorkspace(filename, asAdministrator),
            });
            executor.execute();
        }
    }
}
