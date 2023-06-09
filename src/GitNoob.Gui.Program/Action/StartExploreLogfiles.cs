using GitNoob.Utils;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace GitNoob.Gui.Program.Action
{
    public class StartExploreLogfiles : Action
    {
        public StartExploreLogfiles(ProgramWorkingDirectory Config) : base(Config) { }

        public static IEnumerable<string> GetPaths(ProgramWorkingDirectory config)
        {
            if (config.ProjectWorkingDirectory.ProjectType == null) return null;

            var paths = config.ProjectWorkingDirectory.ProjectType.GetLogfilesPaths();
            if (paths == null) return null;

            var result = new List<string>();
            foreach (var path in paths)
            {
                result.Add(Path.Combine(config.ProjectWorkingDirectory.Path.ToString(), path));
            }

            if (result.Count == 0) return null;

            return result;
        }

        public override bool isStartable()
        {
            return (GetPaths(config) != null);
        }

        public override Icon icon()
        {
            return Resources.getIcon("open logfiles");
        }

        public override void execute()
        {
            if (!isStartable()) return;

            var paths = GetPaths(config);
            foreach (var path in paths)
            {
                Utils.BatFile.StartWindowsExplorer(path, config.Project, config.ProjectWorkingDirectory, config.PhpIni);
            }
        }
    }
}
