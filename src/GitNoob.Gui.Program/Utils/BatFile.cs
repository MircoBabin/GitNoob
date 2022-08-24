using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GitNoob.Gui.Program.Utils
{
    public class BatFile: Config.IExecutor
    {
        public enum RunAsType { runAsInvoker, runAsAdministrator }
        public enum WindowType { showWindow, hideWindow }

        private string _name;
        private RunAsType _runAs;
        private WindowType _window;
        private string _windowTitle;
        private Config.Project _project;
        private Config.WorkingDirectory _projectWorkingDirectory;
        private ConfigFileTemplate.PhpIni _phpIni;
        private string _batWorkingDirectory;

        private StringBuilder _contents;

        public static void StartWindowsExplorer(string openWithPath, 
            Config.Project project, Config.WorkingDirectory projectworkingdirectory, ConfigFileTemplate.PhpIni phpIni)
        {
            if (!openWithPath.EndsWith("\\")) openWithPath = openWithPath + "\\";

            var bat = new BatFile(
                "windows-explorer", RunAsType.runAsInvoker, WindowType.hideWindow, "GitNoob - start Windows Explorer",
                project, projectworkingdirectory, phpIni, openWithPath);
            bat.AppendLine("start \"\" \"" + openWithPath + "\"");
            bat.Start();
        }

        public static void StartExecutable(string exeFilename, string commandLine,
            Config.Project project, Config.WorkingDirectory projectworkingdirectory, ConfigFileTemplate.PhpIni phpIni)
        {
            var bat = new BatFile(
                "executable", RunAsType.runAsInvoker, WindowType.hideWindow, "GitNoob - start executable",
                project, projectworkingdirectory, phpIni);
            bat.AppendLine("start \"\" \"" + exeFilename + "\"" + (!string.IsNullOrWhiteSpace(commandLine) ? " " + commandLine : ""));
            bat.Start();
        }

        public static void StartWebBrowser(string url,
            Config.Project project, Config.WorkingDirectory projectworkingdirectory, ConfigFileTemplate.PhpIni phpIni)
        {
            var bat = new BatFile(
                "url", RunAsType.runAsInvoker, WindowType.hideWindow, "GitNoob - start browser",
                project, projectworkingdirectory, phpIni);
            bat.AppendLine("start \"\" \"" + url + "\"");
            bat.Start();
        }

        public BatFile(string name, RunAsType runAs, WindowType window, string windowTitle,
            Config.Project project, Config.WorkingDirectory projectworkingdirectory,
            ConfigFileTemplate.PhpIni phpIni,
            string batWorkingDirectory = null)
        {
            _name = name;
            _runAs = runAs;
            _window = window;
            _windowTitle = windowTitle;
            _project = project;
            _projectWorkingDirectory = projectworkingdirectory;
            _phpIni = phpIni;
            _batWorkingDirectory = batWorkingDirectory;

            _contents = new StringBuilder();

        }

        private bool NeedsPhp()
        {
            return (_projectWorkingDirectory.ProjectType != null &&
                    _projectWorkingDirectory.ProjectType.Capabilities.NeedsPhp);
        }

        public void Clear()
        {
            _contents.Clear();
        }

        public void Append(string text)
        {
            _contents.Append(text);
        }

        public void AppendLine(string line)
        {
            _contents.AppendLine(line);
        }

        private string WriteBatFile()
        {
            StringBuilder contents = new StringBuilder();
            contents.AppendLine("@echo off");
            contents.AppendLine("chcp 65001 >nul 2>&1"); //Set UTF-8 codepage
            contents.AppendLine("title " + _windowTitle);
            if (!String.IsNullOrWhiteSpace(_batWorkingDirectory))
                contents.AppendLine("cd /D \"" + _batWorkingDirectory + "\"");
            else
                contents.AppendLine("cd /D \"" + _projectWorkingDirectory.Path.ToString() + "\"");

            if (NeedsPhp())
            {
                contents.AppendLine("set PHPRC=" + _phpIni.IniPath); /* Directory containing php.ini */
                contents.AppendLine("path %path%;" + _projectWorkingDirectory.Php.Path.ToString());

                //Global Composer bin directory, e.g. for php-cs-fixer
                //    %appdata%\Composer\\vendor\bin
                string composerGlobalBin = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Composer\\vendor\\bin");
                if (Directory.Exists(composerGlobalBin))
                {
                    contents.AppendLine("path %path%;" + composerGlobalBin);
                }
            }

            if (!string.IsNullOrEmpty(_projectWorkingDirectory.Ngrok.AuthToken))
            {
                contents.AppendLine("set NGROK_AUTHTOKEN=" + _projectWorkingDirectory.Ngrok.AuthToken);
            }

            if (!string.IsNullOrEmpty(_projectWorkingDirectory.Ngrok.ApiKey))
            {
                contents.AppendLine("set NGROK_API_KEY=" + _projectWorkingDirectory.Ngrok.ApiKey);
            }

            contents.AppendLine();
            contents.Append(_contents);

            string batFileContents = contents.ToString();

            string filename = "ExecuteBatFile.";
            if (!string.IsNullOrWhiteSpace(_name)) filename += _name + ".";
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                //Hexadecimal output
                filename += BitConverter.ToString(sha1.ComputeHash(Encoding.UTF8.GetBytes(batFileContents))).Replace("-", string.Empty);
            }
            filename += ".bat";

            var path = FileUtils.TempDirectoryForProject(_project, _projectWorkingDirectory);
            var batFile = Path.Combine(path, filename);
            File.WriteAllText(batFile, batFileContents.ToString(), new UTF8Encoding(false));

            return batFile;
        }

        public void Start()
        {
            var batFile = WriteBatFile();

            var info = new ProcessStartInfo
            {
                FileName = batFile,

                UseShellExecute = true,
            };

            if (_runAs == RunAsType.runAsAdministrator)
            {
                info.Verb = "runas";
            }

            switch(_window)
            {
                case WindowType.hideWindow:
                    info.CreateNoWindow = true;
                    info.WindowStyle = ProcessWindowStyle.Hidden;
                    break;
            }

            Process.Start(info);
        }

        public string GetPhpExe()
        {
            if (!NeedsPhp())
            {
                throw new Exception("PHP configuration is not active");
            }

            return Path.Combine(_projectWorkingDirectory.Php.Path.ToString(), "php.exe");
        }

        public Config.IExecutorResult ExecuteBatFile(string batFileContents)
        {
            if (_runAs != RunAsType.runAsInvoker)
            {
                throw new Exception("BatFile.RunWithConsoleExecutor can only be used with runAsInvoker");
            }

            if (_window != WindowType.hideWindow)
            {
                throw new Exception("BatFile.RunWithConsoleExecutor can only be used with hideWindow");
            }

            Clear();
            Append(batFileContents);

            var batFile = WriteBatFile();

            string workingDirectory;
            if (!String.IsNullOrWhiteSpace(_batWorkingDirectory))
                workingDirectory = _batWorkingDirectory;
            else
                workingDirectory = _projectWorkingDirectory.Path.ToString();


            var console = new Git.Command.ConsoleExecutor(batFile, String.Empty, workingDirectory, null, null);
            console.WaitFor();

            return new Config.IExecutorResult()
            {
                ExitCode = console.ExitCode,
                StandardOutput = console.Output,
                StandardError = console.Error,
            };
        }
    }
}