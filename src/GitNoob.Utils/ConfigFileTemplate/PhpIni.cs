using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GitNoob.Utils.ConfigFileTemplate
{
    public class PhpIni
    {
        public string IniPath
        {
            get
            {
                write();
                return _path;
            }
        }

        public string IniFullFilename
        {
            get
            {
                write();
                return _fullFilename;
            }
        }

        private string _path;
        private string _fullFilename;

        private Config.Project _project;
        private Config.WorkingDirectory _projectWorkingDirectory;

        public PhpIni(Config.Project project, Config.WorkingDirectory projectWorkingDirectory)
        {
            _project = project;
            _projectWorkingDirectory = projectWorkingDirectory;

            _path = FileUtils.TempDirectoryForProject(_project, _projectWorkingDirectory);
            _fullFilename = Path.Combine(_path, "php.ini");
        }

        private void write()
        {
            string GIT_ROOT_DIR = _projectWorkingDirectory.Path.ToString();

            var extdir = Path.Combine(_projectWorkingDirectory.Php.Path.ToString(), "ext");

            string tempDirectory = _projectWorkingDirectory.Php.TempPath.ToString();
            if (_projectWorkingDirectory.ProjectType != null)
                tempDirectory = _projectWorkingDirectory.ProjectType.OverridePhpTempPath(tempDirectory, GIT_ROOT_DIR);
            if (string.IsNullOrWhiteSpace(tempDirectory) || !Directory.Exists(tempDirectory))
                tempDirectory = null;

            string logDirectory = _projectWorkingDirectory.Php.LogPath.ToString();
            if (_projectWorkingDirectory.ProjectType != null)
                logDirectory = _projectWorkingDirectory.ProjectType.OverridePhpLogPath(logDirectory, GIT_ROOT_DIR);
            if (string.IsNullOrWhiteSpace(logDirectory) || !Directory.Exists(logDirectory))
                logDirectory = null;

            string errorLogFilename = null;
            if (!string.IsNullOrWhiteSpace(logDirectory))
                errorLogFilename = Path.Combine(logDirectory, "php-errors.log");

            var contents = _projectWorkingDirectory.Php.PhpIniTemplateFilename.ReadAllText();
            contents = FileUtils.TemplateToContents(contents, _project, _projectWorkingDirectory,
                new Dictionary<string, string>()
                {
                    { "GIT_ROOT_DIR", GIT_ROOT_DIR },
                    { "GIT_ROOT_DIR_SLASH",  GIT_ROOT_DIR.Replace('\\', '/') },

                    { "PHP_EXTENSION_DIR", extdir },
                    { "PHP_EXTENSION_DIR_SLASH", extdir.Replace('\\', '/') },

                    { "PHP_TEMP_DIR", (tempDirectory != null ? tempDirectory : string.Empty) },
                    { "PHP_TEMP_DIR_SLASH", (tempDirectory != null ? tempDirectory.Replace('\\', '/') : string.Empty) },

                    { "PHP_LOG_DIR", (logDirectory != null ? logDirectory : string.Empty) },
                    { "PHP_LOG_DIR_SLASH", (logDirectory != null ? logDirectory.Replace('\\', '/') : string.Empty) },

                    { "PHP_ERROR_LOG_FILENAME", (errorLogFilename != null ? errorLogFilename : string.Empty) },
                    { "PHP_ERROR_LOG_FILENAME_SLASH", (errorLogFilename != null ? errorLogFilename.Replace('\\', '/') : string.Empty) },
                });

            /*
             * GitNoob assumption
             *

            ; Directory in which the loadable extensions (modules) reside.
            ; http://php.net/extension-dir
            ; extension_dir = "./"
            ; On windows:
            extension_dir = "[PHP_EXTENSION_DIR_SLASH]"




            ; Directory where the temporary files should be placed.
            ; Defaults to the system default (see sys_get_temp_dir)
            sys_temp_dir = "[PHP_TEMP_DIR_SLASH]"




            ; Log errors to specified file. PHP's default behavior is to leave this value
            ; empty.
            ; https://php.net/error-log
            ; Example:
            error_log = "[PHP_ERROR_LOG_FILENAME_SLASH]"




            [xdebug]
            ; Install php_xdebug.dll via the instructions at https://xdebug.org/wizard
            ;
            ; Uncomment the next line to enable the xdebug extension.
            ; zend_extension=xdebug

            xdebug.log="[PHP_ERROR_LOG_DIR_SLASH]/php-xdebug.log"
            xdebug.output_dir="[PHP_ERROR_LOG_DIR_SLASH]"

            xdebug.trace_output_name=php-xdebug-trace.%u.%r
            xdebug.start_with_request=yes
            xdebug.mode=trace
            xdebug.trace_format=0
            xdebug.trace_options=0
            xdebug.use_compression=false

            */

            File.WriteAllText(_fullFilename, contents, Encoding.UTF8);
        }
    }
}
