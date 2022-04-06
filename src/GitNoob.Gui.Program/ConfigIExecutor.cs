using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GitNoob.Gui.Program
{
    public class ConfigIExecutor : Config.IExecutor
    {
        private bool _needsPhp;
        private string _phpExePath;
        private string _phpIniPath;
        private string _tempDirectory;
        private string _workingDirectory;

        public ConfigIExecutor(bool NeedsPhp, string PhpExePath, string PhpIniPath, string TempDirectory, string WorkingDirectory)
        {
            _needsPhp = NeedsPhp;
            _phpExePath = PhpExePath;
            _phpIniPath = PhpIniPath;
            _tempDirectory = TempDirectory;
            _workingDirectory = WorkingDirectory;
        }

        public string GetPhpExe()
        {
            return Path.Combine(_phpExePath, "php.exe");
        }

        private string WriteBatFile(string batFileContents)
        {
            string batFile;

            var path = _tempDirectory;
            string filename;
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                //Hexadecimal output
                filename = BitConverter.ToString(sha1.ComputeHash(Encoding.UTF8.GetBytes(batFileContents))).Replace("-", string.Empty);
            }

            batFile = Path.Combine(path, "ExecuteBatFile." + filename + ".bat");
            File.WriteAllText(batFile, batFileContents, Encoding.ASCII);

            return batFile;
        }

        public Config.IExecutorResult ExecuteBatFile(string batFileContents, string commandline = null)
        {
            string batFile = WriteBatFile(batFileContents);

            var paths = new List<string>();
            var envs = new Dictionary<string, string>();
            if (_needsPhp)
            {
                envs.Add("PHPRC", _phpIniPath); /* Directory containing php.ini */
                paths.Add(_phpExePath);

                //Global Composer bin directory, e.g. for php-cs-fixer
                //    %appdata%\Composer\\vendor\bin
                string composerGlobalBin = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Composer\\vendor\\bin");
                if (Directory.Exists(composerGlobalBin))
                {
                    paths.Add(composerGlobalBin);
                }
            }

            var console = new Git.Command.ConsoleExecutor(batFile, commandline, _workingDirectory, null, null, paths, envs);
            console.WaitFor();

            return new Config.IExecutorResult()
            {
                ExitCode = console.ExitCode,
                StandardOutput = console.Output,
                StandardError = console.Error,
            };
        }

        public void OpenBatFileInNewWindow(string batFileContents, string commandline = null)
        {
            string batFile = WriteBatFile(batFileContents);

            var _appendToPath = new List<string>();
            var _environmentVariables = new Dictionary<string, string>();
            if (_needsPhp)
            {
                _appendToPath.Add(_phpExePath);
                _environmentVariables.Add("PHPRC", _phpIniPath); /* Directory containing php.ini */
            };

            var _process = new Process();
            _process.StartInfo.FileName = batFile;
            _process.StartInfo.WorkingDirectory = _workingDirectory;
            _process.StartInfo.Arguments = commandline;
            _process.StartInfo.UseShellExecute = false;

            if (_appendToPath != null)
            {
                string result = _process.StartInfo.EnvironmentVariables["Path"];
                foreach (var path in _appendToPath)
                {
                    result += ";" + path;
                }
                _process.StartInfo.EnvironmentVariables["Path"] = result;
            }

            if (_environmentVariables != null)
            {
                foreach (var item in _environmentVariables)
                {
                    _process.StartInfo.EnvironmentVariables[item.Key] = item.Value;
                }
            }

            _process.Start();
        }
    }
}
