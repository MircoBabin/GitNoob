using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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

        private string GetFullPath(string maybeRelativePath, string baseDirectory)
        {
            if (String.IsNullOrWhiteSpace(maybeRelativePath))
                return (new DirectoryInfo(Path.GetFullPath(baseDirectory))).FullName;

            var root = Path.GetPathRoot(maybeRelativePath);
            if (string.IsNullOrEmpty(root))
                return (new DirectoryInfo(Path.GetFullPath(Path.Combine(baseDirectory, maybeRelativePath)))).FullName;
            if (root == "\\")
                return (new DirectoryInfo(Path.GetFullPath(Path.Combine(Path.GetPathRoot(baseDirectory), maybeRelativePath.Remove(0, 1))))).FullName;
            return (new DirectoryInfo(maybeRelativePath)).FullName;
        }

        private string replaceValueVariables(string value)
        {
            value = value.Replace("%prjPath%", _prjPath);
            value = value.Replace("%binPath%", _binPath);
            value = value.Replace("%myDocuments%", _myDocuments);
            value = value.Replace("%gitnoob%", _gitNoobProgramPath);

            return value;
        }

        private string ReadValue(IniFileParser ini, string Section, string Key)
        {
            string value = ini.ReadValue(Section, Key).Trim();

            return replaceValueVariables(value);
        }

        private void ReadPath(IniFileParser ini, string Section, string Key, ConfigPath intoValue, string baseDirectory = null)
        {
            string value = ReadValue(ini, Section, Key);
            if (String.IsNullOrWhiteSpace(value)) return;

            if (value.Contains("%gitRoot%"))
            {
                intoValue.CopyFrom(new ConfigPath(value));
                return;
            }

            if (baseDirectory == null) baseDirectory = Path.GetDirectoryName(ini.IniFilename);
            intoValue.CopyFrom(new ConfigPath(GetFullPath(value, baseDirectory)));
        }

        private void ReadFilename(IniFileParser ini, string Section, string Key, ConfigFilename intoValue)
        {
            string value = ReadValue(ini, Section, Key);
            if (String.IsNullOrWhiteSpace(value)) return;

            if (value.Contains("%gitRoot%"))
            {
                intoValue.CopyFrom(new ConfigFilename(value));
                return;
            }

            intoValue.CopyFrom(new ConfigFilename(GetFullPath(value, Path.GetDirectoryName(ini.IniFilename))));
        }

        private void ReadBoolean(IniFileParser ini, string Section, string Key, ConfigBoolean intoValue)
        {
            string value = ReadValue(ini, Section, Key);
            if (String.IsNullOrWhiteSpace(value)) return;

            value = value.ToLowerInvariant();
            if (value == "true") intoValue.CopyFrom(new ConfigBoolean(true));
            else if (value == "false") intoValue.CopyFrom(new ConfigBoolean(false));
        }

        private void LoadGitnoobIni()
        { 
            Dictionary<string, System.Action<IniFileParser, string, string>> sectionDefinitions = new Dictionary<string, System.Action<IniFileParser, string, string>>()
            {
                {  "apache-", (ini, sectionname, name) =>
                    {
                        if (_globalApaches.ContainsKey(name))
                            _globalApaches.Remove(name);

                        _globalApaches.Add(name, LoadApache(ini, sectionname, String.Empty));
                    }
                },
                {  "php-", (ini, sectionname, name) =>
                    {
                        if (_globalPhps.ContainsKey(name))
                            _globalPhps.Remove(name);

                        _globalPhps.Add(name, LoadPhp(ini, sectionname, String.Empty));
                    }
                },
                {  "ngrok-", (ini, sectionname, name) =>
                    {
                        if (_globalNgroks.ContainsKey(name))
                            _globalNgroks.Remove(name);

                        _globalNgroks.Add(name, LoadNgrok(ini, sectionname, String.Empty));
                    }
                },
                {  "smtpserver-", (ini, sectionname, name) =>
                    {
                        if (_globalSmtpServers.ContainsKey(name))
                            _globalSmtpServers.Remove(name);

                        _globalSmtpServers.Add(name, LoadSmtpServer(ini, sectionname, String.Empty));
                    }
                },
            };

            {
                IniFileParser ini = new IniFileParser(_rootConfigurationIniFilename);
                ConfigPath path = new ConfigPath(null);

                {
                    var loadConfigurationFrom = ReadValue(ini, "gitnoob", "loadRootConfigurationFrom");
                    if (!String.IsNullOrWhiteSpace(loadConfigurationFrom))
                    {
                        loadConfigurationFrom = Path.GetFullPath(loadConfigurationFrom);
                        if (!File.Exists(loadConfigurationFrom))
                            throw new Exception("File " + _rootConfigurationIniFilename + " is invalid. loadRootConfigurationFrom setting does not point to an existing file: " + loadConfigurationFrom);

                        _rootConfigurationIniFilename = loadConfigurationFrom;
                        ini = new IniFileParser(_rootConfigurationIniFilename);
                    }
                }

                ReadPath(ini, "gitnoob", "prjPath", path);
                _prjPath = path.ToString();
                if (_prjPath.EndsWith("\\")) _prjPath = _prjPath.Substring(0, _prjPath.Length - 1);

                ReadPath(ini, "gitnoob", "binPath", path);
                _binPath = path.ToString();
                if (_binPath.EndsWith("\\")) _binPath = _binPath.Substring(0, _binPath.Length - 1);

                foreach (var item in ini.GetSectionKeysAndValues("projecttypes"))
                {
                    if (item.Key.ToLowerInvariant().Trim() == "assembly")
                    {
                        var assemblyFilename = GetFullPath(replaceValueVariables(item.Value), _gitNoobProgramPath);
                        ProjectTypeLoader.LoadProjectTypesAssembly(assemblyFilename);
                    }
                }

                _globalGitCommitName = ReadValue(ini, "gitnoob", "commitname");
                _globalGitCommitEmail = ReadValue(ini, "gitnoob", "commitemail");
                _globalApacheName = ReadValue(ini, "gitnoob", "apache");
                _globalPhpName = ReadValue(ini, "gitnoob", "php");
                _globalNgrokName = ReadValue(ini, "gitnoob", "ngrok");
                _globalSmtpServerName = ReadValue(ini, "gitnoob", "smtpserver");

                foreach (string sectionname in ini.GetSectionNames())
                {
                    foreach (var item in sectionDefinitions)
                    {
                        string sectionStart = item.Key;
                        System.Action<IniFileParser, string, string> action = item.Value;

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
                        var inifilename = GetFullPath(replaceValueVariables(item.Value), Path.GetDirectoryName(ini.IniFilename));
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

        private WorkingGit LoadGit(IniFileParser ini, string Section, string gitName)
        {
            WorkingGit git = new WorkingGit();
            string value;

            if (!String.IsNullOrWhiteSpace(gitName))
            {
                if (_globalGits.ContainsKey(gitName))
                {
                    git.CopyFrom(_globalGits[gitName]);
                }
            }

            value = ReadValue(ini, Section, "origin");
            if (!String.IsNullOrWhiteSpace(value)) git.RemoteUrl = value;

            value = ReadValue(ini, Section, "mainbranch");
            if (!String.IsNullOrWhiteSpace(value)) git.MainBranch = value;

            value = ReadValue(ini, Section, "commitname");
            if (!String.IsNullOrWhiteSpace(value)) git.CommitName = value;
            if (String.IsNullOrWhiteSpace(git.CommitName)) git.CommitName = _globalGitCommitName;

            value = ReadValue(ini, Section, "commitemail");
            if (!String.IsNullOrWhiteSpace(value)) git.CommitEmail = value;
            if (String.IsNullOrWhiteSpace(git.CommitEmail)) git.CommitEmail = _globalGitCommitEmail;

            return git;
        }

        private Apache LoadApache(IniFileParser ini, string Section, string apacheName)
        {
            Apache apache = new Apache();
            string value;

            value = ReadValue(ini, Section, "apache");
            if (!String.IsNullOrWhiteSpace(value)) apacheName = value;
            if (!String.IsNullOrWhiteSpace(apacheName))
            {
                if (_globalApaches.ContainsKey(apacheName))
                {
                    apache.CopyFrom(_globalApaches[apacheName]);
                }
            }

            ReadPath(ini, Section, "apachePath", apache.ApachePath);
            ReadFilename(ini, Section, "apacheConf", apache.ApacheConfTemplateFilename);

            ReadBoolean(ini, Section, "apacheUseSsl", apache.UseSsl);
            ReadFilename(ini, Section, "apacheSslCertificateKeyFile", apache.SslCertificateKeyFile);
            ReadFilename(ini, Section, "apacheSslCertificateFile", apache.SslCertificateFile);
            ReadFilename(ini, Section, "apacheSslCertificateChainFile", apache.SslCertificateChainFile);

            return apache;
        }

        private Php LoadPhp(IniFileParser ini, string Section, string phpName)
        {
            Php php = new Php();
            string value;

            value = ReadValue(ini, Section, "php");
            if (!String.IsNullOrWhiteSpace(value)) phpName = value;
            if (!String.IsNullOrWhiteSpace(phpName))
            {
                if (_globalPhps.ContainsKey(phpName))
                {
                    php.CopyFrom(_globalPhps[phpName]);
                }
            }

            ReadPath(ini, Section, "phpPath", php.Path);
            ReadFilename(ini, Section, "phpIni", php.PhpIniTemplateFilename);

            return php;
        }

        private Ngrok LoadNgrok(IniFileParser ini, string Section, string ngrokName)
        {
            Ngrok ngrok = new Ngrok();
            string value;

            value = ReadValue(ini, Section, "ngrok");
            if (!String.IsNullOrWhiteSpace(value)) ngrokName = value;
            if (!String.IsNullOrWhiteSpace(ngrokName))
            {
                if (_globalNgroks.ContainsKey(ngrokName))
                {
                    ngrok.CopyFrom(_globalNgroks[ngrokName]);
                }
            }

            ReadPath(ini, Section, "ngrokPath", ngrok.NgrokPath);

            value = ReadValue(ini, Section, "ngrokPort");
            if (!string.IsNullOrWhiteSpace(value))
            {
                try
                {
                    ngrok.Port = Convert.ToInt32(value);
                }
                catch { }
            }

            ReadFilename(ini, Section, "ngrokAgentConfigurationFile", ngrok.AgentConfigurationFile);

            value = ReadValue(ini, Section, "ngrokAuthToken");
            if (!string.IsNullOrWhiteSpace(value)) ngrok.AuthToken = value;

            value = ReadValue(ini, Section, "ngrokApiKey");
            if (!string.IsNullOrWhiteSpace(value)) ngrok.ApiKey = value;

            return ngrok;
        }

        private SmtpServer LoadSmtpServer(IniFileParser ini, string Section, string smtpServerName)
        {
            SmtpServer smtpserver = new SmtpServer();
            string value;

            value = ReadValue(ini, Section, "smtpserver");
            if (!String.IsNullOrWhiteSpace(value)) smtpServerName = value;
            if (!String.IsNullOrWhiteSpace(smtpServerName))
            {
                if (_globalSmtpServers.ContainsKey(smtpServerName))
                {
                    smtpserver.CopyFrom(_globalSmtpServers[smtpServerName]);
                }
            }

            ReadFilename(ini, Section, "smtpServerExecutable", smtpserver.Executable);

            return smtpserver;
        }

        private Webpage LoadWebpage(IniFileParser ini, string Section, string webpageName)
        {
            Webpage webpage = new Webpage();
            string value;

            value = String.Empty; //Is not nameable
            if (!String.IsNullOrWhiteSpace(value)) webpageName = value;
            if (!String.IsNullOrWhiteSpace(webpageName))
            {
                if (_globalWebpages.ContainsKey(webpageName))
                {
                    webpage.CopyFrom(_globalWebpages[webpageName]);
                }
            }

            value = ReadValue(ini, Section, "homepage");
            if (!string.IsNullOrWhiteSpace(value)) webpage.Homepage = value;

            return webpage;
        }

        private Project LoadProject(string inifilename)
        {
            IniFileParser ini = new IniFileParser(inifilename);

            var project = new Project();

            project.Name = ReadValue(ini, "gitnoob", "name");
            project.ProjectType = ProjectTypeLoader.Load(ReadValue(ini, "gitnoob", "type"));
            ReadFilename(ini, "gitnoob", "icon", project.IconFilename);

            string gitName;
            {
                string name = "gitnoob-project-" + inifilename;
                _globalGits.Add(name, LoadGit(ini, "gitnoob", String.Empty));

                gitName = name;
            }

            string apacheName;
            {
                string name = "gitnoob-project-" + inifilename;
                _globalApaches.Add(name, LoadApache(ini, "gitnoob", _globalApacheName));

                apacheName = name;
            }

            string phpName;
            {
                string name = "gitnoob-project-" + inifilename;
                _globalPhps.Add(name, LoadPhp(ini, "gitnoob", _globalPhpName));

                phpName = name;
            }

            string ngrokName;
            {
                string name = "gitnoob-project-" + inifilename;
                _globalNgroks.Add(name, LoadNgrok(ini, "gitnoob", _globalNgrokName));

                ngrokName = name;
            }

            string smtpServerName;
            {
                string name = "gitnoob-project-" + inifilename;
                _globalSmtpServers.Add(name, LoadSmtpServer(ini, "gitnoob", _globalSmtpServerName));

                smtpServerName = name;
            }

            string webpageName;
            {
                string name = "gitnoob-project-" + inifilename;
                _globalWebpages.Add(name, LoadWebpage(ini, "gitnoob", String.Empty));

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

                        project.WorkingDirectories.Add(name, LoadWorkingDirectory(project, ini, sectionname, 
                            gitName, webpageName, apacheName, phpName, ngrokName, smtpServerName));
                    }
                    catch { }
                }
            }

            return project;
        }

        private WorkingDirectory LoadWorkingDirectory(Project project, IniFileParser ini, string Section, 
            string gitName, string webpageName, string apacheName, string phpName, string ngrokName, string smtpServerName)
        {
            WorkingDirectory WorkingDirectory = new WorkingDirectory();
            string value;

            WorkingDirectory.Name = ReadValue(ini, Section, "name");

            ReadPath(ini, Section, "path", WorkingDirectory.Path);

            ReadFilename(ini, Section, "icon", WorkingDirectory.IconFilename);
            ReadFilename(ini, Section, "image", WorkingDirectory.ImageFilename);
            WorkingDirectory.ImageBackgroundColor = ReadValue(ini, Section, "imagebackgroundcolor");

            WorkingDirectory.ProjectType = ProjectTypeLoader.Load(ReadValue(ini, Section, "type"));
            if (WorkingDirectory.ProjectType == null) WorkingDirectory.ProjectType = project.ProjectType;

            WorkingDirectory.Git = LoadGit(ini, Section, gitName);

            WorkingDirectory.Webpage = LoadWebpage(ini, Section, webpageName);

            WorkingDirectory.Apache = LoadApache(ini, Section, apacheName);

            WorkingDirectory.Php = LoadPhp(ini, Section, phpName);

            WorkingDirectory.Ngrok = LoadNgrok(ini, Section, ngrokName);

            WorkingDirectory.SmtpServer = LoadSmtpServer(ini, Section, smtpServerName);


            value = ReadValue(ini, Section, "port");
            if (!string.IsNullOrWhiteSpace(value))
            {
                try
                {
                    WorkingDirectory.Apache.Port = Convert.ToInt32(value);
                }
                catch { }
            }

            ReadPath(ini, Section, "webroot", WorkingDirectory.Apache.WebrootPath, WorkingDirectory.Path.ToString()); //webroot is relative to %gitRoot%
            if (WorkingDirectory.Apache.WebrootPath.isEmpty())
            {
                WorkingDirectory.Apache.WebrootPath.CopyFrom(WorkingDirectory.Path);
            }

            ReadFilename(ini, Section, "workspace", WorkingDirectory.Editor.WorkspaceFilename);
            ReadBoolean(ini, Section, "workspace-run-as-administrator", WorkingDirectory.Editor.WorkspaceRunAsAdministrator);

            WorkingDirectory.useWorkingDirectory();

            return WorkingDirectory;
        }
    }
}
