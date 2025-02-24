using System;
using System.Collections.Concurrent;
using System.IO;

namespace GitNoob.Git.Command.WorkingTree
{
    public class IsGitRootDirectory : Command
    {
        private static ConcurrentDictionary<string, bool> _resolved = new ConcurrentDictionary<string, bool>();
        public static void ClearCache()
        {
            _resolved.Clear();
        }

        private string _resolveName;
        public bool? result { get; private set; }

        public IsGitRootDirectory(GitWorkingDirectory gitworkingdirectory, bool useCache = true) : base(gitworkingdirectory)
        {
            _resolveName = gitworkingdirectory.WorkingPath;

            if (!useCache || !_resolved.ContainsKey(_resolveName))
            {
                result = null;
                RunGit("toplevel", new string[] { "rev-parse", "--show-toplevel" });
                RunGit("not-inside-other-repository", new string[] { "rev-parse", "--show-toplevel" }, Path.Combine(_gitworkingdirectory.WorkingPath, ".."));
            }
            else
            {
                result = _resolved[_resolveName];
            }
        }

        protected override void RunGitDone()
        {
            if (result == null)
            {
                {
                    var cmd = GetGitExecutor("toplevel");
                    var toplevel = cmd.Output.Trim().Replace('/', '\\');

                    result = (toplevel.ToLowerInvariant() == _gitworkingdirectory.WorkingPath.ToLowerInvariant());
                }

                if (result == true)
                {
                    var cmd = GetGitExecutor("not-inside-other-repository");
                    var toplevel = cmd.Output.Trim().Replace('/', '\\');
                    if (!String.IsNullOrEmpty(toplevel) && Directory.Exists(toplevel)) result = false;
                }

                _resolved[_resolveName] = (result == true);
            }
        }
    }
}
