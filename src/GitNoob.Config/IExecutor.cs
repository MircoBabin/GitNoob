using System;

namespace GitNoob.Config
{
    public class IExecutorResult
    {
        public int ExitCode { get; set; }
        public string StandardOutput { get; set; }
        public string StandardError { get; set; }

        public IExecutorResult()
        {
            this.ExitCode = -1;
            this.StandardOutput = String.Empty;
            this.StandardError = String.Empty;
        }

        public IExecutorResult(int ExitCode, string StandardOutput, string StandardError)
        {
            this.ExitCode = ExitCode;
            this.StandardOutput = StandardOutput;
            this.StandardError = StandardError;
        }
    }

    public interface IExecutor
    {
        string GetPhpExe();
        IExecutorResult ExecuteBatFile(string batFileContents, string commandline = null);

        void OpenBatFileInNewWindow(string batFileContents, string commandline = null);
    }
}
