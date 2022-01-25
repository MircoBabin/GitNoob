using System.Collections.Concurrent;
using System.IO;

namespace GitNoob.Git.Command.Config
{
    public class GetCredentialHelper : Command
    {
        private static ConcurrentDictionary<string, string> _resolved = new ConcurrentDictionary<string, string>();
        public static void ClearCache()
        {
            _resolved.Clear();
        }

        private string _resolveName;
        public string result { get; private set; }

        public GetCredentialHelper(GitWorkingDirectory gitworkingdirectory) : base(gitworkingdirectory)
        {
            _resolveName = gitworkingdirectory.WorkingPath;

            if (!_resolved.ContainsKey(_resolveName))
            {
                result = null;
                RunGit("credential", "config credential.helper");
            }
            else
            {
                result = _resolved[_resolveName];
            }
        }

        public bool TryCommandGet()
        {
            if (string.IsNullOrWhiteSpace(result)) return false;

            if (!File.Exists(result)) return false;

            using (var executor = new ConsoleExecutor(result, "get", _gitworkingdirectory.WorkingPath, null, null))
            {
                executor.WriteToStandardInput("\u001a"); //ctrl-z
                executor.CloseStandardInput();
                executor.WaitFor();
                return (executor.ExitCode == 0);
            }
        }

        protected override void RunGitDone()
        {
            if (result == null)
            {
                var credential = GetGitExecutor("credential");

                result = credential.Output.Trim().Replace('/', '\\');
                _resolved[_resolveName] = result;
            }
        }
    }
}
