using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GitNoob.Utils
{
    public class BatFile : Config.IExecutor
    {
        public enum RunAsType { runAsInvoker, runAsAdministrator }
        public enum WindowType { showWindow, hideWindow, showWindowWithPauseAtEnd }

        public delegate void OnErrorCallback(Exception ex);

        private OnErrorCallback _onError;
        private string _name;
        private RunAsType _runAs;
        private WindowType _window;
        private string _windowTitle;
        private Config.Project _project;
        private Config.WorkingDirectory _projectWorkingDirectory;
        private ConfigFileTemplate.PhpIni _phpIni;
        private string _batWorkingDirectory;

        private StringBuilder _contents;

        public static void StartWindowsExplorer(OnErrorCallback onError,
            string openWithPath,
            Config.Project project, Config.WorkingDirectory projectworkingdirectory, ConfigFileTemplate.PhpIni phpIni)
        {
            if (!openWithPath.EndsWith("\\")) openWithPath = openWithPath + "\\";

            var bat = new BatFile(onError,
                "windows-explorer", RunAsType.runAsInvoker, WindowType.hideWindow,
                "GitNoob - start Windows Explorer",
                project, projectworkingdirectory, phpIni, openWithPath);
            bat.AppendLine("%SystemRoot%\\explorer.exe \"" + openWithPath + "\"");
            bat.Start();
        }

        public static void StartDosPrompt(OnErrorCallback onError,
            bool asAdministrator, Config.Project project, Config.WorkingDirectory projectworkingdirectory, ConfigFileTemplate.PhpIni phpIni)
        {
            var bat = new BatFile(onError,
                "executable", (asAdministrator ? RunAsType.runAsAdministrator : RunAsType.runAsInvoker), WindowType.showWindow,
                FileUtils.DeriveFilename(String.Empty, project.Name) + "-" + FileUtils.DeriveFilename(String.Empty, projectworkingdirectory.Name),
                project, projectworkingdirectory, phpIni);

            bat.StartAndKeepDosPromptOpen();
        }

        public static void StartExecutable(OnErrorCallback onError,
            string exeFilename, string commandLine,
            Config.Project project, Config.WorkingDirectory projectworkingdirectory, ConfigFileTemplate.PhpIni phpIni)
        {
            var bat = new BatFile(onError,
                "executable", RunAsType.runAsInvoker, WindowType.hideWindow,
                "GitNoob - start executable",
                project, projectworkingdirectory, phpIni);
            bat.AppendLine("start \"\" \"" + exeFilename + "\"" + (!string.IsNullOrWhiteSpace(commandLine) ? " " + commandLine : ""));
            bat.Start();
        }

        public static void StartWebBrowser(OnErrorCallback onError,
            string url,
            Config.Project project, Config.WorkingDirectory projectworkingdirectory, ConfigFileTemplate.PhpIni phpIni)
        {
            var bat = new BatFile(onError,
                "url", RunAsType.runAsInvoker, WindowType.hideWindow,
                "GitNoob - start browser",
                project, projectworkingdirectory, phpIni);
            bat.AppendLine("start \"\" \"" + url + "\"");
            bat.Start();
        }

        private static string _cacheEditorExecutable = null;
        private static bool _cacheEditorExecutableMultipleFiles = false;
        private static Tuple<string, bool> GetEditorExecutable()
        {
            if (_cacheEditorExecutable == null)
            {
                try
                {
                    _cacheEditorExecutable = FileUtils.FindExeInProgramFiles("Notepad++\\notepad++.exe");
                    _cacheEditorExecutableMultipleFiles = true;
                }
                catch { }
            }

            if (_cacheEditorExecutable == null)
            {
                try
                {
                    _cacheEditorExecutable = FileUtils.FindExePath("notepad.exe");
                    _cacheEditorExecutableMultipleFiles = false;
                }
                catch { }
            }

            if (_cacheEditorExecutable == null) throw new Exception("Notepad++ or notepad is not found.");

            return new Tuple<string, bool>(_cacheEditorExecutable, _cacheEditorExecutableMultipleFiles);
        }
        public static void StartEditor(OnErrorCallback onError,
            IEnumerable<string> files)
        {
            StartEditor(onError, files, null, null, null);
        }

        public static void StartEditor(OnErrorCallback onError,
            IEnumerable<string> files,
            Config.Project project, Config.WorkingDirectory projectworkingdirectory, ConfigFileTemplate.PhpIni phpIni)
        {
            string exeFilename;
            bool multipleFiles;
            try
            {
                var setting = GetEditorExecutable();
                exeFilename = setting.Item1;
                multipleFiles = setting.Item2;
            }
            catch (Exception ex)
            {
                if (onError != null)
                    onError(ex);
                else
                    throw;
                return;
            }

            if (multipleFiles)
            {
                StringBuilder cmdline = new StringBuilder();
                foreach (var file in files)
                {
                    cmdline.Append(" \"");
                    cmdline.Append(file);
                    cmdline.Append("\"");
                }
                string commandLine = cmdline.ToString();

                var bat = new BatFile(onError,
                    "executable", RunAsType.runAsInvoker, WindowType.hideWindow,
                    "GitNoob - start editor",
                    project, projectworkingdirectory, phpIni);
                bat.AppendLine("start \"\" \"" + exeFilename + "\"" + (!string.IsNullOrWhiteSpace(commandLine) ? " " + commandLine : ""));
                bat.Start();
            }
            else
            {
                foreach (var file in files)
                {
                    string commandLine = " \"" + file + "\"";

                    var bat = new BatFile(onError,
                        "executable", RunAsType.runAsInvoker, WindowType.hideWindow,
                        "GitNoob - start editor",
                        project, projectworkingdirectory, phpIni);
                    bat.AppendLine("start \"\" \"" + exeFilename + "\"" + (!string.IsNullOrWhiteSpace(commandLine) ? " " + commandLine : ""));
                    bat.Start();
                }
            }
        }

        public BatFile(OnErrorCallback onError,
            string name, RunAsType runAs, WindowType window, string windowTitle,
            Config.Project project, Config.WorkingDirectory projectworkingdirectory,
            ConfigFileTemplate.PhpIni phpIni,
            string batWorkingDirectory = null)
        {
            _onError = onError;
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
            var path = FileUtils.TempDirectoryForProject(_project, _projectWorkingDirectory);

            StringBuilder contents = new StringBuilder();
            contents.AppendLine("@echo off");
            contents.AppendLine("chcp 65001 >nul 2>&1"); //Set UTF-8 codepage
            contents.AppendLine("title " + _windowTitle);
            if (!String.IsNullOrWhiteSpace(_batWorkingDirectory))
                contents.AppendLine("cd /D \"" + _batWorkingDirectory + "\"");
            else if (_projectWorkingDirectory != null)
                contents.AppendLine("cd /D \"" + _projectWorkingDirectory.Path.ToString() + "\"");
            else
                contents.AppendLine("cd /D \"%~dp0\"");

            if (_projectWorkingDirectory != null)
            {
                if (NeedsPhp())
                {
                    string phpPath = FileUtils.GetExactPathName(_projectWorkingDirectory.Php.Path.ToString());

                    contents.AppendLine("set PHPRC=" + _phpIni.IniPath); /* Directory containing php.ini */

                    var environmentPathParts = FileUtils.RemoveExeFromEnvironmentPath("php.exe");
                    environmentPathParts.Add(phpPath);

                    //Global Composer bin directory, e.g. for php-cs-fixer
                    //    %appdata%\Composer\\vendor\bin
                    string composerGlobalBin = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Composer\\vendor\\bin");
                    if (Directory.Exists(composerGlobalBin))
                    {
                        environmentPathParts.Add(composerGlobalBin);
                    }

                    //Rebuild path
                    bool first = true;
                    foreach(var part in environmentPathParts)
                    {
                        if (first)
                        {
                            contents.AppendLine("set GitNoob_TmpPath=" + part);
                            first = false;
                        }
                        else
                        {
                            contents.AppendLine("set GitNoob_TmpPath=%GitNoob_TmpPath%;" + part);
                        }
                    }
                    contents.AppendLine("path %GitNoob_TmpPath%");
                    contents.AppendLine("set GitNoob_TmpPath=");
                }

                if (!string.IsNullOrEmpty(_projectWorkingDirectory.Ngrok.AuthToken))
                {
                    contents.AppendLine("set NGROK_AUTHTOKEN=" + _projectWorkingDirectory.Ngrok.AuthToken);
                }

                if (!string.IsNullOrEmpty(_projectWorkingDirectory.Ngrok.ApiKey))
                {
                    contents.AppendLine("set NGROK_API_KEY=" + _projectWorkingDirectory.Ngrok.ApiKey);
                }
            }

            contents.AppendLine();
            contents.Append(_contents);

            switch(_window)
            {
                case WindowType.showWindowWithPauseAtEnd:
                    contents.AppendLine();
                    contents.AppendLine("pause");
                    break;
            }

            string batFileContents = contents.ToString();

            string filename = "ExecuteBatFile.";
            if (!string.IsNullOrWhiteSpace(_name)) filename += _name + ".";
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                //Hexadecimal output
                filename += BitConverter.ToString(sha1.ComputeHash(Encoding.UTF8.GetBytes(batFileContents))).Replace("-", string.Empty);
            }
            filename += ".bat";

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

            switch (_window)
            {
                case WindowType.hideWindow:
                    info.CreateNoWindow = true;
                    info.WindowStyle = ProcessWindowStyle.Hidden;
                    break;
            }

            try
            {
                Process.Start(info);
            }
            catch (Exception ex)
            {
                if (_onError != null)
                    _onError(ex);
                else
                    throw;
            }
        }

        public void StartAndKeepDosPromptOpen()
        {
            switch(_window)
            {
                case WindowType.showWindow:
                case WindowType.showWindowWithPauseAtEnd:
                    break;

                default:
                    throw new Exception("BatFile.StartAndKeepDosPromptOpen can only be used with showWindow");
            }

            var orgTitle = _windowTitle;
            string batFile;
            try
            {
                if (_runAs == RunAsType.runAsAdministrator)
                {
                    _windowTitle = "Administrator: " + _windowTitle;
                }
                batFile = WriteBatFile();
            }
            finally
            {
                _windowTitle = orgTitle;
            }

            var exe = FileUtils.FindExePath("%ComSpec%");
            var info = new ProcessStartInfo
            {
                FileName = exe,
                Arguments = "/K \"" + batFile + "\"",

                UseShellExecute = true,
            };

            if (_runAs == RunAsType.runAsAdministrator)
            {
                info.Verb = "runas";
            }

            try
            {
                Process.Start(info);
            }
            catch (Exception ex)
            {
                if (_onError != null)
                    _onError(ex);
                else
                    throw;
            }
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
                throw new Exception("BatFile.ExecuteBatFile - ConsoleExecutor can only be used with runAsInvoker");
            }

            if (_window != WindowType.hideWindow)
            {
                throw new Exception("BatFile.ExecuteBatFile - ConsoleExecutor can only be used with hideWindow");
            }

            Clear();
            Append(batFileContents);

            var batFile = WriteBatFile();

            string workingDirectory;
            if (!String.IsNullOrWhiteSpace(_batWorkingDirectory))
                workingDirectory = _batWorkingDirectory;
            else
                workingDirectory = _projectWorkingDirectory.Path.ToString();


            var console = new ConsoleExecutor(batFile, String.Empty, workingDirectory, null, null);
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