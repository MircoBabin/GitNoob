using System.Collections.Concurrent;
using System.IO;

namespace GitNoob.Git.Command.WorkingTree
{
    public class ResolveGitPath : Command
    {
        private static ConcurrentDictionary<string, string> _resolved = new ConcurrentDictionary<string, string>();
        public static void ClearCache()
        {
            _resolved.Clear();
        }

        private string _resolveName;
        public string result { get; private set; }

        public ResolveGitPath(GitWorkingDirectory gitworkingdirectory, string path) : base(gitworkingdirectory)
        {
            _resolveName = gitworkingdirectory.WorkingPath + " : " + path;

            if (!_resolved.ContainsKey(_resolveName))
            {
                result = null;
                RunGit("resolve", new string[] { "rev-parse", "--no-flags", "--git-path", path });
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
                var resolve = GetGitExecutor("resolve");

                result = Path.Combine(_gitworkingdirectory.WorkingPath, resolve.Output.Trim().Replace('/', '\\'));
                _resolved[_resolveName] = result;
            }
        }
    }
}
