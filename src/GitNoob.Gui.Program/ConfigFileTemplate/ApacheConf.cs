using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GitNoob.Gui.Program.ConfigFileTemplate
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
        private bool _written = false;

        private Config.Project _project;
        private Config.WorkingDirectory _projectWorkingDirectory;
        private PhpIni _phpIni;

        public ApacheConf(Config.Project project, Config.WorkingDirectory projectWorkingDirectory, PhpIni phpIni)
        {
            _project = project;
            _projectWorkingDirectory = projectWorkingDirectory;
            _phpIni = phpIni;

            ProjectnameASCII = Utils.FileUtils.DeriveFilename(String.Empty, _project.Name) + "-" + Utils.FileUtils.DeriveFilename(String.Empty, _projectWorkingDirectory.Name);

            if (!String.IsNullOrWhiteSpace(_projectWorkingDirectory.Apache.ApachePath))
            {
                _path = Path.Combine(_projectWorkingDirectory.Apache.ApachePath, "conf");
                _fullFilename = Path.Combine(_path, "httpd." + ProjectnameASCII + ".conf");

                PidFullFilename = Path.Combine(_projectWorkingDirectory.Apache.ApachePath, "logs", "httpd." + ProjectnameASCII + ".pid");
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
            if (_written && File.Exists(_fullFilename)) return;

            string php_loadmodule = null;
            string dll;

            dll = Path.Combine(_projectWorkingDirectory.Php.Path, "php8apache2_4.dll");
            if (String.IsNullOrEmpty(php_loadmodule) && File.Exists(dll))
            {
                php_loadmodule = "LoadModule php_module \"" + dll.Replace('\\', '/') + "\"";
            }

            dll = Path.Combine(_projectWorkingDirectory.Php.Path, "php7apache2_4.dll");
            if (String.IsNullOrEmpty(php_loadmodule) && File.Exists(dll))
            {
                php_loadmodule = "LoadModule php7_module \"" + dll.Replace('\\', '/') + "\"";
            }

            dll = Path.Combine(_projectWorkingDirectory.Php.Path, "php5apache2_4.dll");
            if (String.IsNullOrEmpty(php_loadmodule) && File.Exists(dll))
            {
                php_loadmodule = "LoadModule php5_module \"" + dll.Replace('\\', '/') + "\"";
            }

            var contents = Utils.FileUtils.TemplateToContents(_projectWorkingDirectory.Apache.ApacheConfTemplateContents, _project, _projectWorkingDirectory,
                new Dictionary<string, string>()
                {
                        { "APACHE_PORT", _projectWorkingDirectory.Apache.Port.ToString() },
                        { "APACHE_SRVROOT_SLASH", _projectWorkingDirectory.Apache.ApachePath.Replace('\\', '/') },
                        { "APACHE_PIDFILE_SLASH", PidFullFilename.Replace('\\', '/') },
                        { "APACHE_ERRORLOG", "logs/error." + ProjectnameASCII + ".log" },
                        { "APACHE_CUSTOMLOG", "logs/access." + ProjectnameASCII + ".log" },

                        { "PROJECTNAME", ProjectnameASCII },

                        { "WEBROOT", _projectWorkingDirectory.Apache.WebrootPath },
                        { "WEBROOT_SLASH", _projectWorkingDirectory.Apache.WebrootPath.Replace('\\', '/') },

                        { "PHP_PATH_SLASH", _projectWorkingDirectory.Php.Path.Replace('\\', '/') },
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
            _written = true;
        }
    }
}
