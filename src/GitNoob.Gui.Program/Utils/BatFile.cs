using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GitNoob.Gui.Program.Utils
{
    public class BatFile
    {
        private string _name;
        private bool _runAsAdministrator;
        private Config.Project _project;
        private Config.WorkingDirectory _projectWorkingDirectory;
        private ConfigFileTemplate.PhpIni _phpIni;

        private StringBuilder _contents;

        public BatFile(string name, bool runAsAdministrator,
            Config.Project project, Config.WorkingDirectory projectworkingdirectory,
            ConfigFileTemplate.PhpIni phpIni)
        {
            _name = name;
            _runAsAdministrator = runAsAdministrator;
            _project = project;
            _projectWorkingDirectory = projectworkingdirectory;
            _phpIni = phpIni;

            _contents = new StringBuilder();
            _contents.AppendLine("@echo off");
            _contents.AppendLine("chcp 65001 >nul 2>&1"); //Set UTF-8 codepage
        }

        public void AppendLine(string line)
        {
            _contents.AppendLine(line);
        }

        public void Execute()
        {
            string batFileContents = _contents.ToString();

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

            var info = new ProcessStartInfo
            {
                FileName = batFile,
                WorkingDirectory = _projectWorkingDirectory.Path.ToString(),

                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
            };

            if (_runAsAdministrator)
            {
                info.Verb = "runas";
            }

            FileUtils.Execute_ProcessStartInfo(info, _projectWorkingDirectory, _phpIni);
        }
    }
}