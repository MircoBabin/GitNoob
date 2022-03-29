using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace GitNoob.Gui.Program.Action
{
    public class StartExploreLogfiles : Action, IAction
    {
        public StartExploreLogfiles(StepsExecutor.StepConfig Config) : base(Config) { }

        public static IEnumerable<string> GetPaths(StepsExecutor.StepConfig stepConfig)
        {
            if (stepConfig.Config.ProjectWorkingDirectory.ProjectType == null) return null;

            var paths = stepConfig.Config.ProjectWorkingDirectory.ProjectType.GetLogfilesPaths();
            if (paths == null) return null;

            var result = new List<string>();
            foreach (var path in paths)
            {
                result.Add(Path.Combine(stepConfig.Config.ProjectWorkingDirectory.Path.ToString(), path));
            }

            if (result.Count == 0) return null;

            return result;
        }

        public bool isStartable()
        {
            return (GetPaths(stepConfig) != null);
        }

        public Icon icon()
        {
            return Utils.Resources.getIcon("open logfiles");
        }

        public void execute()
        {
            if (!isStartable()) return;

            var paths = GetPaths(stepConfig);
            foreach (var path in paths)
            {
                if (path.EndsWith("\\"))
                    System.Diagnostics.Process.Start(path);
                else
                    System.Diagnostics.Process.Start(path + "\\");
            }
        }
    }
}
