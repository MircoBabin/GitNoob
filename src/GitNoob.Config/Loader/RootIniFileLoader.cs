using System;
using System.Collections.Generic;
using System.IO;

namespace GitNoob.Config.Loader
{
    public class RootIniFileLoader : IConfig
    {
        private string _rootConfigurationIniFilename;

        private string _gitNoobProgramPath;
        private string _myDocuments;
        private string _binPath;
        private string _prjPath;

        private string _globalGitCommitName;
        private string _globalGitCommitEmail;
        private string _globalPhpName;
        private string _globalApacheName;
        private string _globalNgrokName;
        private string _globalSmtpServerName;
        private Dictionary<string, WorkingGit> _globalGits;
        private Dictionary<string, Apache> _globalApaches;
        private Dictionary<string, Php> _globalPhps;
        private Dictionary<string, Ngrok> _globalNgroks;
        private Dictionary<string, SmtpServer> _globalSmtpServers;
        private Dictionary<string, Webpage> _globalWebpages;

        private Dictionary<string, Project> _projects;

        public RootIniFileLoader(string RootConfigurationIniFilename, string GitNoobProgramPath)
        {
            _rootConfigurationIniFilename = Path.GetFullPath(RootConfigurationIniFilename);

            _gitNoobProgramPath = Path.GetFullPath(GitNoobProgramPath);
            if (_gitNoobProgramPath.EndsWith("\\")) _gitNoobProgramPath = _gitNoobProgramPath.Substring(0, _gitNoobProgramPath.Length - 1);

            _myDocuments = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            if (_myDocuments.EndsWith("\\")) _myDocuments = _myDocuments.Substring(0, _myDocuments.Length - 1);

            _binPath = String.Empty;
            _prjPath = String.Empty;

            _globalGitCommitName = String.Empty;
            _globalGitCommitEmail = String.Empty;
            _globalPhpName = String.Empty;
            _globalApacheName = String.Empty;
            _globalNgrokName = String.Empty;
            _globalSmtpServerName = String.Empty;
            _globalGits = new Dictionary<string, WorkingGit>();
            _globalApaches = new Dictionary<string, Apache>();
            _globalPhps = new Dictionary<string, Php>();
            _globalNgroks = new Dictionary<string, Ngrok>();
            _globalSmtpServers = new Dictionary<string, SmtpServer>();
            _globalWebpages = new Dictionary<string, Webpage>();

            _projects = new Dictionary<string, Project>();

            LoadGitnoobIni();
        }

        public IEnumerable<Project> GetProjects()
        {
            return _projects.Values;
        }

        private GitnoobIniFileReader CreateReader(string IniFilename)
        {
            return new GitnoobIniFileReader(IniFilename,
                _gitNoobProgramPath, _myDocuments, _binPath, _prjPath,
                _globalGitCommitName,
                _globalGitCommitEmail,
                _globalGits,
                _globalApaches,
                _globalPhps,
                _globalNgroks,
                _globalSmtpServers,
                _globalWebpages);
        }

        private void LoadGitnoobIni()
        { 
            Dictionary<string, System.Action<GitnoobIniFileReader, string, string>> sectionDefinitions = new Dictionary<string, System.Action<GitnoobIniFileReader, string, string>>()
            {
                {  "apache-", (ini, sectionname, name) =>
                    {
                        if (_globalApaches.ContainsKey(name))
                            _globalApaches.Remove(name);

                        _globalApaches.Add(name, ini.LoadApache(sectionname, String.Empty));
                    }
                },
                {  "php-", (ini, sectionname, name) =>
                    {
                        if (_globalPhps.ContainsKey(name))
                            _globalPhps.Remove(name);

                        _globalPhps.Add(name, ini.LoadPhp(sectionname, String.Empty));
                    }
                },
                {  "ngrok-", (ini, sectionname, name) =>
                    {
                        if (_globalNgroks.ContainsKey(name))
                            _globalNgroks.Remove(name);

                        _globalNgroks.Add(name, ini.LoadNgrok(sectionname, String.Empty));
                    }
                },
                {  "smtpserver-", (ini, sectionname, name) =>
                    {
                        if (_globalSmtpServers.ContainsKey(name))
                            _globalSmtpServers.Remove(name);

                        _globalSmtpServers.Add(name, ini.LoadSmtpServer(sectionname, String.Empty));
                    }
                },
            };

            {
                GitnoobIniFileReader ini = CreateReader(_rootConfigurationIniFilename);
                ConfigPath path = new ConfigPath(null);

                {
                    var loadConfigurationFrom = ini.ReadString("gitnoob", "loadRootConfigurationFrom");
                    if (!String.IsNullOrWhiteSpace(loadConfigurationFrom))
                    {
                        loadConfigurationFrom = Path.GetFullPath(loadConfigurationFrom);
                        if (!File.Exists(loadConfigurationFrom))
                            throw new Exception("File " + _rootConfigurationIniFilename + " is invalid. loadRootConfigurationFrom setting does not point to an existing file: " + loadConfigurationFrom);

                        _rootConfigurationIniFilename = loadConfigurationFrom;
                        ini = CreateReader(_rootConfigurationIniFilename);
                    }
                }

                ini.ReadPath("gitnoob", "prjPath", path);
                _prjPath = path.ToString();
                if (_prjPath.EndsWith("\\")) _prjPath = _prjPath.Substring(0, _prjPath.Length - 1);
                ini.SetPrjPath(_prjPath);

                ini.ReadPath("gitnoob", "binPath", path);
                _binPath = path.ToString();
                if (_binPath.EndsWith("\\")) _binPath = _binPath.Substring(0, _binPath.Length - 1);
                ini.SetBinPath(_binPath);

                foreach (var item in ini.GetSectionKeysAndValues("projecttypes"))
                {
                    if (item.Key.ToLowerInvariant().Trim() == "assembly")
                    {
                        var assemblyFilename = ini.GetFullPath(ini.replaceValueVariables(item.Value), _gitNoobProgramPath);
                        ProjectTypeLoader.LoadProjectTypesAssembly(assemblyFilename);
                    }
                }

                _globalGitCommitName = ini.ReadString("gitnoob", "commitname");
                ini.SetGitCommitName(_globalGitCommitName);

                _globalGitCommitEmail = ini.ReadString("gitnoob", "commitemail");
                ini.SetGitCommitEmail(_globalGitCommitEmail);

                _globalApacheName = ini.ReadString("gitnoob", "apache");
                _globalPhpName = ini.ReadString("gitnoob", "php");
                _globalNgrokName = ini.ReadString("gitnoob", "ngrok");
                _globalSmtpServerName = ini.ReadString("gitnoob", "smtpserver");

                foreach (string sectionname in ini.GetSectionNames())
                {
                    foreach (var item in sectionDefinitions)
                    {
                        string sectionStart = item.Key;
                        System.Action<GitnoobIniFileReader, string, string> action = item.Value;

                        if (sectionname.StartsWith(sectionStart) && sectionname.Length > sectionStart.Length)
                        {
                            try
                            {
                                var name = sectionname.Substring(sectionStart.Length).Trim();
                                if (name.Length > 0 && !name.StartsWith("gitnoob-") && 
                                    name != "none" && name != "null" && name != "empty")
                                {
                                    action(ini, sectionname, name);
                                }
                            }
                            catch { }
                        }
                    }
                }

                foreach (var item in ini.GetSectionKeysAndValues("projects"))
                {
                    if (item.Key.ToLowerInvariant().Trim() == "project")
                    {
                        var inifilename = ini.GetFullPath(ini.replaceValueVariables(item.Value), Path.GetDirectoryName(ini.IniFilename));
                        if (!_projects.ContainsKey(inifilename) && File.Exists(inifilename))
                        {
                            try
                            {
                                _projects.Add(inifilename, LoadProject(inifilename));
                            }
                            catch { }
                        }
                    }
                }
            }
        }

        public Project LoadProject(string inifilename)
        {
            GitnoobIniFileReader ini = CreateReader(inifilename);

            var project = new Project();

            project.Name = ini.ReadString("gitnoob", "name");
            project.ProjectType = ProjectTypeLoader.Load(ini.ReadString("gitnoob", "type"));
            ini.ReadFilename("gitnoob", "icon", project.IconFilename);

            string gitName;
            {
                string name = "gitnoob-project-" + inifilename;
                _globalGits.Add(name, ini.LoadGit("gitnoob", String.Empty));

                gitName = name;
            }

            string apacheName;
            {
                string name = "gitnoob-project-" + inifilename;
                _globalApaches.Add(name, ini.LoadApache("gitnoob", _globalApacheName));

                apacheName = name;
            }

            string phpName;
            {
                string name = "gitnoob-project-" + inifilename;
                _globalPhps.Add(name, ini.LoadPhp("gitnoob", _globalPhpName));

                phpName = name;
            }

            string ngrokName;
            {
                string name = "gitnoob-project-" + inifilename;
                _globalNgroks.Add(name, ini.LoadNgrok("gitnoob", _globalNgrokName));

                ngrokName = name;
            }

            string smtpServerName;
            {
                string name = "gitnoob-project-" + inifilename;
                _globalSmtpServers.Add(name, ini.LoadSmtpServer("gitnoob", _globalSmtpServerName));

                smtpServerName = name;
            }

            string webpageName;
            {
                string name = "gitnoob-project-" + inifilename;
                _globalWebpages.Add(name, ini.LoadWebpage("gitnoob", String.Empty));

                webpageName = name;
            }

            foreach (string sectionname in ini.GetSectionNames())
            {
                if (sectionname.StartsWith("working-"))
                {
                    try
                    {
                        var name = sectionname.Substring(8).Trim();

                        if (project.WorkingDirectories.ContainsKey(name))
                            project.WorkingDirectories.Remove(name);

                        project.WorkingDirectories.Add(name, ini.LoadWorkingDirectory(project, sectionname,
                            gitName, webpageName, apacheName, phpName, ngrokName, smtpServerName));
                    }
                    catch { }
                }
            }

            return project;
        }
    }
}
