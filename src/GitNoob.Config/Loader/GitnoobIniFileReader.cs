using System;
using System.Collections.Generic;
using System.IO;

namespace GitNoob.Config.Loader
{
    internal class GitnoobIniFileReader
    {
        private IniFileParser _parser;

        private string _gitNoobProgramPath;
        private string _myDocuments;
        private string _binPath;
        private string _prjPath;

        private string _globalGitCommitName;
        private string _globalGitCommitEmail;

        private Dictionary<string, WorkingGit> _globalGits;
        private Dictionary<string, Apache> _globalApaches;
        private Dictionary<string, Php> _globalPhps;
        private Dictionary<string, Ngrok> _globalNgroks;
        private Dictionary<string, SmtpServer> _globalSmtpServers;
        private Dictionary<string, Webpage> _globalWebpages;

        public GitnoobIniFileReader(string IniFilename, 
            string GitNoobProgramPath, string MyDocuments, string BinPath, string PrjPath,
            string GlobalGitCommitName,
            string GlobalGitCommitEmail,
            Dictionary<string, WorkingGit> GlobalGits,
            Dictionary<string, Apache> GlobalApaches,
            Dictionary<string, Php> GlobalPhps,
            Dictionary<string, Ngrok> GlobalNgroks,
            Dictionary<string, SmtpServer> GlobalSmtpServers,
            Dictionary<string, Webpage> GlobalWebpages)
        {
            _parser = new IniFileParser(IniFilename);

            _gitNoobProgramPath = GitNoobProgramPath;
            _myDocuments = MyDocuments;

            _globalGitCommitName = GlobalGitCommitName;
            _globalGitCommitEmail = GlobalGitCommitEmail;

            _globalGits = GlobalGits;
            _globalApaches = GlobalApaches;
            _globalPhps = GlobalPhps;
            _globalNgroks = GlobalNgroks;
            _globalSmtpServers = GlobalSmtpServers;
            _globalWebpages = GlobalWebpages;

            SetBinPath(BinPath);
            SetPrjPath(PrjPath);
        }

        public void SetBinPath(string BinPath)
        {
            _binPath = BinPath;
            if (_binPath == null) _binPath = String.Empty;
        }

        public void SetPrjPath(string PrjPath)
        {
            _prjPath = PrjPath;
            if (_prjPath == null) _prjPath = String.Empty;
        }

        public void SetGitCommitName(string value)
        {
            _globalGitCommitName = value;
            if (_globalGitCommitName == null) _globalGitCommitName = String.Empty;
        }

        public void SetGitCommitEmail(string value)
        {
            _globalGitCommitEmail = value;
            if (_globalGitCommitEmail == null) _globalGitCommitEmail = String.Empty;
        }

        public string IniFilename { get { return _parser.IniFilename; } }

        public List<string> GetSectionNames()
        {
            return _parser.GetSectionNames();
        }

        public List<KeyValue> GetSectionKeysAndValues(string Section)
        {
            return _parser.GetSectionKeysAndValues(Section);
        }

        #region Read values
        public string GetFullPath(string maybeRelativePath, string baseDirectory)
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

        public string replaceValueVariables(string value)
        {
            value = value.Replace("%prjPath%", _prjPath);
            value = value.Replace("%binPath%", _binPath);
            value = value.Replace("%myDocuments%", _myDocuments);
            value = value.Replace("%gitnoob%", _gitNoobProgramPath);

            return value;
        }

        public string ReadString(IniFileParser parser, string Section, string Key)
        {
            string value = parser.ReadValue(Section, Key).Trim();

            return replaceValueVariables(value);

        }

        public string ReadString(string Section, string Key)
        {
            return ReadString(_parser, Section, Key);
        }

        public void ReadPath(IniFileParser parser, string Section, string Key, ConfigPath intoValue, string baseDirectory = null)
        {
            string value = ReadString(parser, Section, Key);
            if (String.IsNullOrWhiteSpace(value)) return;

            if (value.Contains("%gitRoot%"))
            {
                intoValue.CopyFrom(new ConfigPath(value));
                return;
            }

            if (baseDirectory == null) baseDirectory = Path.GetDirectoryName(parser.IniFilename);
            intoValue.CopyFrom(new ConfigPath(GetFullPath(value, baseDirectory)));
        }

        public void ReadPath(string Section, string Key, ConfigPath intoValue, string baseDirectory = null)
        {
            ReadPath(_parser, Section, Key, intoValue, baseDirectory);
        }

        public void ReadFilename(IniFileParser parser, string Section, string Key, ConfigFilename intoValue)
        {
            string value = ReadString(parser, Section, Key);
            if (String.IsNullOrWhiteSpace(value)) return;

            if (value.Contains("%gitRoot%"))
            {
                intoValue.CopyFrom(new ConfigFilename(value));
                return;
            }

            intoValue.CopyFrom(new ConfigFilename(GetFullPath(value, Path.GetDirectoryName(parser.IniFilename))));
        }

        public void ReadFilename(string Section, string Key, ConfigFilename intoValue)
        {
            ReadFilename(_parser, Section, Key, intoValue);
        }

        public void ReadBoolean(IniFileParser parser, string Section, string Key, ConfigBoolean intoValue)
        {
            string value = ReadString(parser, Section, Key);
            if (String.IsNullOrWhiteSpace(value)) return;

            value = value.ToLowerInvariant();
            if (value == "true") intoValue.CopyFrom(new ConfigBoolean(true));
            else if (value == "false") intoValue.CopyFrom(new ConfigBoolean(false));
        }

        public void ReadBoolean(string Section, string Key, ConfigBoolean intoValue)
        {
            ReadBoolean(_parser, Section, Key, intoValue);
        }

        #endregion

        #region Read sections
        public WorkingGit LoadGit(string Section, string gitName)
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

            value = ReadString(Section, "origin");
            if (!String.IsNullOrWhiteSpace(value)) git.RemoteUrl = value;

            value = ReadString(Section, "mainbranch");
            if (!String.IsNullOrWhiteSpace(value)) git.MainBranch = value;

            {
                value = ReadString(Section, "commitname");
                if (!String.IsNullOrWhiteSpace(value)) git.CommitName = value;
                if (String.IsNullOrWhiteSpace(git.CommitName)) git.CommitName = _globalGitCommitName;

                value = ReadString(Section, "commitemail");
                if (!String.IsNullOrWhiteSpace(value)) git.CommitEmail = value;
                if (String.IsNullOrWhiteSpace(git.CommitEmail)) git.CommitEmail = _globalGitCommitEmail;

                ReadBoolean(Section, "commitname-settings-clear-on-exit", git.ClearCommitNameAndEmailOnExit);
                ReadBoolean(Section, "touch-timestamp-of-commits-before-merge", git.TouchTimestampOfCommitsBeforeMerge);
            }

            {
                ConfigFilename other = new ConfigFilename(null);
                ReadFilename(Section, "commitname-settings-via-filename", other);
                string otherFilename = other.ToString();

                if (!String.IsNullOrWhiteSpace(otherFilename) && File.Exists(otherFilename))
                {
                    IniFileParser _otherParser = new IniFileParser(otherFilename);
                    string otherSection = "gitnoob";

                    string commitname = ReadString(_otherParser, otherSection, "commitname");
                    string commitemail = ReadString(_otherParser, otherSection, "commitemail");
                    if (!String.IsNullOrWhiteSpace(commitname) && !String.IsNullOrWhiteSpace(commitemail))
                    {
                        git.CommitName = commitname;
                        git.CommitEmail = commitemail;

                        ReadBoolean(_otherParser, otherSection, "commitname-settings-clear-on-exit", git.ClearCommitNameAndEmailOnExit);
                    }
                }
            }

            return git;
        }

        public Apache LoadApache(string Section, string apacheName)
        {
            Apache apache = new Apache();
            string value;

            value = ReadString(Section, "apache");
            if (!String.IsNullOrWhiteSpace(value)) apacheName = value;
            if (!String.IsNullOrWhiteSpace(apacheName))
            {
                if (_globalApaches.ContainsKey(apacheName))
                {
                    apache.CopyFrom(_globalApaches[apacheName]);
                }
            }

            ReadPath(Section, "apachePath", apache.ApachePath);
            ReadFilename(Section, "apacheConf", apache.ApacheConfTemplateFilename);

            ReadBoolean(Section, "apacheUseSsl", apache.UseSsl);
            ReadFilename(Section, "apacheSslCertificateKeyFile", apache.SslCertificateKeyFile);
            ReadFilename(Section, "apacheSslCertificateFile", apache.SslCertificateFile);
            ReadFilename(Section, "apacheSslCertificateChainFile", apache.SslCertificateChainFile);

            return apache;
        }

        public Php LoadPhp(string Section, string phpName)
        {
            Php php = new Php();
            string value;

            value = ReadString(Section, "php");
            if (!String.IsNullOrWhiteSpace(value)) phpName = value;
            if (!String.IsNullOrWhiteSpace(phpName))
            {
                if (_globalPhps.ContainsKey(phpName))
                {
                    php.CopyFrom(_globalPhps[phpName]);
                }
            }

            ReadPath(Section, "phpPath", php.Path);
            ReadPath(Section, "phpTempPath", php.TempPath);
            ReadPath(Section, "phpLogPath", php.LogPath);
            ReadFilename(Section, "phpIni", php.PhpIniTemplateFilename);

            return php;
        }

        public Ngrok LoadNgrok(string Section, string ngrokName)
        {
            Ngrok ngrok = new Ngrok();
            string value;

            value = ReadString(Section, "ngrok");
            if (!String.IsNullOrWhiteSpace(value)) ngrokName = value;
            if (!String.IsNullOrWhiteSpace(ngrokName))
            {
                if (_globalNgroks.ContainsKey(ngrokName))
                {
                    ngrok.CopyFrom(_globalNgroks[ngrokName]);
                }
            }

            ReadPath(Section, "ngrokPath", ngrok.NgrokPath);

            value = ReadString(Section, "ngrokPort");
            if (!string.IsNullOrWhiteSpace(value))
            {
                try
                {
                    ngrok.Port = Convert.ToInt32(value);
                }
                catch { }
            }

            ReadFilename(Section, "ngrokAgentConfigurationFile", ngrok.AgentConfigurationFile);

            value = ReadString(Section, "ngrokAuthToken");
            if (!string.IsNullOrWhiteSpace(value)) ngrok.AuthToken = value;

            value = ReadString(Section, "ngrokApiKey");
            if (!string.IsNullOrWhiteSpace(value)) ngrok.ApiKey = value;

            return ngrok;
        }

        public SmtpServer LoadSmtpServer(string Section, string smtpServerName)
        {
            SmtpServer smtpserver = new SmtpServer();
            string value;

            value = ReadString(Section, "smtpserver");
            if (!String.IsNullOrWhiteSpace(value)) smtpServerName = value;
            if (!String.IsNullOrWhiteSpace(smtpServerName))
            {
                if (_globalSmtpServers.ContainsKey(smtpServerName))
                {
                    smtpserver.CopyFrom(_globalSmtpServers[smtpServerName]);
                }
            }

            ReadFilename(Section, "smtpServerExecutable", smtpserver.Executable);

            return smtpserver;
        }

        public Webpage LoadWebpage(string Section, string webpageName)
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

            value = ReadString(Section, "homepage");
            if (!string.IsNullOrWhiteSpace(value)) webpage.Homepage = value;

            return webpage;
        }

        public WorkingDirectory LoadWorkingDirectory(Project project, string Section,
            string gitName, string webpageName, string apacheName, string phpName, string ngrokName, string smtpServerName)
        {
            WorkingDirectory WorkingDirectory = new WorkingDirectory(project.projectConfigurationIniFilename);
            string value;

            WorkingDirectory.Name = ReadString(Section, "name");

            ReadPath(Section, "path", WorkingDirectory.Path);

            ReadFilename(Section, "icon", WorkingDirectory.IconFilename);
            ReadFilename(Section, "image", WorkingDirectory.ImageFilename);
            WorkingDirectory.ImageBackgroundColor = ReadString(Section, "imagebackgroundcolor");

            WorkingDirectory.ProjectType = ProjectTypeLoader.Load(ReadString(Section, "type"));
            if (WorkingDirectory.ProjectType == null) WorkingDirectory.ProjectType = project.ProjectType;

            WorkingDirectory.Git = LoadGit(Section, gitName);

            WorkingDirectory.Webpage = LoadWebpage(Section, webpageName);

            WorkingDirectory.Apache = LoadApache(Section, apacheName);

            WorkingDirectory.Php = LoadPhp(Section, phpName);

            WorkingDirectory.Ngrok = LoadNgrok(Section, ngrokName);

            WorkingDirectory.SmtpServer = LoadSmtpServer(Section, smtpServerName);


            value = ReadString(Section, "port");
            if (!string.IsNullOrWhiteSpace(value))
            {
                try
                {
                    WorkingDirectory.Apache.Port = Convert.ToInt32(value);
                }
                catch { }
            }

            ReadPath(Section, "webroot", WorkingDirectory.Apache.WebrootPath, WorkingDirectory.Path.ToString()); //webroot is relative to %gitRoot%
            if (WorkingDirectory.Apache.WebrootPath.isEmpty())
            {
                WorkingDirectory.Apache.WebrootPath.CopyFrom(WorkingDirectory.Path);
            }

            ReadFilename(Section, "workspace", WorkingDirectory.Editor.WorkspaceFilename);
            ReadBoolean(Section, "workspace-run-as-administrator", WorkingDirectory.Editor.WorkspaceRunAsAdministrator);

            WorkingDirectory.useWorkingDirectory();

            return WorkingDirectory;
        }

        #endregion
    }
}
