using System.Collections.Generic;

namespace GitNoob.Git.Command
{
    public class ExecutorGit
    {
        protected string _gitExecutable;
        protected string _workingDirectory;
        protected string _parms;
        protected Dictionary<string, string> _environmentVariables;
        protected GitNoob.Utils.ConsoleExecutor _executor = null;

        public ExecutorGit(string gitExecutable, string workingDirectory, string parms, Dictionary<string,string> environmentVariables)
        {
            _gitExecutable = gitExecutable;
            _workingDirectory = workingDirectory;
            _parms = parms;
            _environmentVariables = environmentVariables;

            Execute();
        }

        private void Execute()
        {
            _executor = new GitNoob.Utils.ConsoleExecutor(_gitExecutable, _parms, _workingDirectory, null, null, null, _environmentVariables);
            _executor.CloseStandardInput();
        }

        public string WaitFor()
        {
            return _executor.WaitFor();
        }

        public int ExitCode
        {
            get
            {
                return _executor.ExitCode;
            }
        }

        public string Input
        {
            get
            {
                return _executor.Input;
            }
        }

        public string Output
        {
            get
            {
                return _executor.Output;
            }
        }

        public string Error
        {
            get
            {
                return _executor.Error;
            }
        }

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
        }

        ~ExecutorGit()
        {
            Dispose(false);
        }

        protected bool _isDisposed = false;
        protected void Dispose(bool disposing)
        {
            if (_isDisposed) return;
            _isDisposed = true;

            if (_executor != null)
            {
                _executor.Dispose();
                _executor = null;
            }
        }
        #endregion
    }
}
