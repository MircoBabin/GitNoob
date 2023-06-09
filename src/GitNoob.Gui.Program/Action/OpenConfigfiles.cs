using GitNoob.Utils;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace GitNoob.Gui.Program.Action
{
    public class OpenConfigfiles : Action
    {
        public OpenConfigfiles(ProgramWorkingDirectory Config) : base(Config) { }

        public IEnumerable<string> GetFiles()
        {
            if (config.ProjectWorkingDirectory.ProjectType == null) return null;

            var files = config.ProjectWorkingDirectory.ProjectType.GetConfigurationFiles();
            if (files == null) return null;

            var result = new List<string>();
            foreach(var file in files)
            {
                result.Add(Path.Combine(config.ProjectWorkingDirectory.Path.ToString(), file));
            }

            if (result.Count == 0) return null;

            return result;
        }

        public override bool isStartable()
        {
            return (GetFiles() != null);
        }

        public override Icon icon()
        {
            return Resources.getIcon("open configfiles");
        }

        public override void execute()
        {
            if (!isStartable()) return;

            if (config.Visualizer.isFrontendLocked()) return;

            var executor = new StepsExecutor.StepsExecutor(config, new List<StepsExecutor.IExecutableByStepsExecutor>()
            {
                new Step.EditFiles(GetFiles()),
            });
            executor.execute();
        }
    }
}
