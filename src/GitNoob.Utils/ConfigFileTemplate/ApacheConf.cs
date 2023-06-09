using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GitNoob.Utils.ConfigFileTemplate
{
    public class ApacheConf
    {
        public string ConfPath
        {
            get
            {
                write();
                return _path;
            }
        }

        public string ConfFullFilename
        {
            get
            {
                write();
                return _fullFilename;
            }
        }

        public string PidFullFilename { get; }
        public string ProjectnameASCII { get; }

        private string _path;
        private string _fullFilename;

        private Config.Project _project;
        private Config.WorkingDirectory _projectWorkingDirectory;
        private PhpIni _phpIni;

        public ApacheConf(Config.Project project, Config.WorkingDirectory projectWorkingDirectory, PhpIni phpIni)
        {
            _project = project;
            _projectWorkingDirectory = projectWorkingDirectory;
            _phpIni = phpIni;

            ProjectnameASCII = FileUtils.DeriveFilename(String.Empty, _project.Name) + "-" + FileUtils.DeriveFilename(String.Empty, _projectWorkingDirectory.Name);

            if (!_projectWorkingDirectory.Apache.ApachePath.isEmpty())
            {
                string apachePath = _projectWorkingDirectory.Apache.ApachePath.ToString();

                _path = Path.Combine(apachePath, "conf");
                _fullFilename = Path.Combine(_path, "httpd." + ProjectnameASCII + ".conf");

                PidFullFilename = Path.Combine(apachePath, "logs", "httpd." + ProjectnameASCII + ".pid");
            }
            else
            {
                _path = null;
                _fullFilename = null;

                PidFullFilename = null;
            }
        }

        private void write()
        {
            string php_loadmodule = null;
            string dll;

            string phpPath = _projectWorkingDirectory.Php.Path.ToString();

            dll = Path.Combine(phpPath, "php8apache2_4.dll");
            if (String.IsNullOrEmpty(php_loadmodule) && File.Exists(dll))
            {
                php_loadmodule = "LoadModule php_module \"" + dll.Replace('\\', '/') + "\"";
            }

            dll = Path.Combine(phpPath, "php7apache2_4.dll");
            if (String.IsNullOrEmpty(php_loadmodule) && File.Exists(dll))
            {
                php_loadmodule = "LoadModule php7_module \"" + dll.Replace('\\', '/') + "\"";
            }

            dll = Path.Combine(phpPath, "php5apache2_4.dll");
            if (String.IsNullOrEmpty(php_loadmodule) && File.Exists(dll))
            {
                php_loadmodule = "LoadModule php5_module \"" + dll.Replace('\\', '/') + "\"";
            }

            string apacheSsl = string.Empty;
            string apacheSslOnOff = "off";
            string apacheSslCertificateKeyFile = _projectWorkingDirectory.Apache.SslCertificateKeyFile.ToString().Replace('\\', '/');
            string apacheSslCertificateFile = _projectWorkingDirectory.Apache.SslCertificateFile.ToString().Replace('\\', '/');
            string apacheSslCertificateChainFile = _projectWorkingDirectory.Apache.SslCertificateChainFile.ToString().Replace('\\', '/');
            if (_projectWorkingDirectory.Apache.UseSsl.Value)
            {
                apacheSslOnOff = "on";
                apacheSsl =
                    "SSLEngine on" + Environment.NewLine +
                    "SSLCertificateKeyFile \"" + apacheSslCertificateKeyFile + "\"" + Environment.NewLine +
                    "SSLCertificateFile \"" + apacheSslCertificateFile + "\"" + Environment.NewLine +
                    "SSLCertificateChainFile \"" + apacheSslCertificateChainFile + "\"" + Environment.NewLine;
            }

            var contents = _projectWorkingDirectory.Apache.ApacheConfTemplateFilename.ReadAllText();
            contents = FileUtils.TemplateToContents(contents, _project, _projectWorkingDirectory,
                new Dictionary<string, string>()
                {
                    { "GIT_ROOT_DIR",  _projectWorkingDirectory.Path.ToString() },
                    { "GIT_ROOT_DIR_SLASH",  _projectWorkingDirectory.Path.ToString().Replace('\\', '/') },

                    { "APACHE_PORT", _projectWorkingDirectory.Apache.Port.ToString() },
                    { "APACHE_SRVROOT_SLASH", _projectWorkingDirectory.Apache.ApachePath.ToString().Replace('\\', '/') },
                    { "APACHE_PIDFILE_SLASH", PidFullFilename.Replace('\\', '/') },
                    { "APACHE_ERRORLOG", "logs/error." + ProjectnameASCII + ".log" },
                    { "APACHE_CUSTOMLOG", "logs/access." + ProjectnameASCII + ".log" },

                    { "APACHE_VIRTUALHOST_SSL", apacheSsl },
                    { "APACHE_VIRTUALHOST_SSL_ONOFF", apacheSslOnOff },
                    { "APACHE_VIRTUALHOST_SSL_CERTIFICATEKEYFILE_SLASH", apacheSslCertificateKeyFile },
                    { "APACHE_VIRTUALHOST_SSL_CERTIFICATEFILE_SLASH", apacheSslCertificateFile },
                    { "APACHE_VIRTUALHOST_SSL_CERTIFICATECHAINFILE_SLASH", apacheSslCertificateChainFile },

                    { "PROJECTNAME", ProjectnameASCII },

                    { "WEBROOT", _projectWorkingDirectory.Apache.WebrootPath.ToString() },
                    { "WEBROOT_SLASH", _projectWorkingDirectory.Apache.WebrootPath.ToString().Replace('\\', '/') },

                    { "PHP_PATH_SLASH", _projectWorkingDirectory.Php.Path.ToString().Replace('\\', '/') },
                    { "PHP_INIFILE_SLASH", _phpIni.IniFullFilename.Replace('\\', '/') },
                    { "APACHE_LOADMODULE_PHP", (php_loadmodule != null ? php_loadmodule : "") },

                    { "COMPUTERNAME", System.Environment.MachineName },
                });

            /*
             * GitNoob assumption
             * 

            [APACHE_LOADMODULE_PHP]
            PHPIniDir "[PHP_INIFILE_SLASH]"

            Define SRVROOT "[APACHE_SRVROOT_SLASH]"
            ServerRoot "${SRVROOT}"
            PidFile "[APACHE_PIDFILE_SLASH]"

            Listen [APACHE_PORT]

            ServerName localhost:[APACHE_PORT]

            ErrorLog "[APACHE_ERRORLOG]"

            CustomLog "[APACHE_CUSTOMLOG]" common

            <VirtualHost *:[APACHE_PORT]>
                ServerName localhost
                ServerAlias [COMPUTERNAME]
                DocumentRoot "[WEBROOT]"
                <Directory "[WEBROOT_SLASH]/">
                    AllowOverride All
                    Options FollowSymLinks Indexes 
            #       Require local
                </Directory>
            </VirtualHost>
            */

            File.WriteAllText(_fullFilename, contents, Encoding.UTF8);
        }
    }
}
