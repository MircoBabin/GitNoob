using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GitNoob.Gui.Program.ConfigFileTemplate
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

            _path = Utils.FileUtils.TempDirectoryForProject(_project, _projectWorkingDirectory);
            _fullFilename = Path.Combine(_path, "php.ini");
        }

        private void write()
        {
            var extdir = Path.Combine(_projectWorkingDirectory.Php.Path.ToString(), "ext");

            var contents = _projectWorkingDirectory.Php.PhpIniTemplateFilename.ReadAllText();
            contents = Utils.FileUtils.TemplateToContents(contents, _project, _projectWorkingDirectory,
                new Dictionary<string, string>()
                {
                    { "GIT_ROOT_DIR",  _projectWorkingDirectory.Path.ToString() },
                    { "GIT_ROOT_DIR_SLASH",  _projectWorkingDirectory.Path.ToString().Replace('\\', '/') },

                    { "PHP_EXTENSION_DIR", extdir },
                    { "PHP_EXTENSION_DIR_SLASH", extdir.Replace('\\', '/') },
                });

            //TODO PHP_LOG_DIR (automatic for Laravel-9)
            //     PHP_TEMP_DIR e.g. for session files

            /*
             * GitNoob assumption
             *

            ; Directory in which the loadable extensions (modules) reside.
            ; http://php.net/extension-dir
            ; extension_dir = "./"
            ; On windows:
            extension_dir = "[PHP_EXTENSION_DIR_SLASH]"



            
            ; Log errors to specified file. PHP's default behavior is to leave this value
            ; empty.
            ; https://php.net/error-log
            ; Example:
            error_log = "[GIT_ROOT_DIR_SLASH]/storage/logs/php-errors.log"
            ; Log errors to syslog (Event Log on Windows).
            ;error_log = syslog




            [xdebug]
            ; Install php_xdebug.dll via the instructions at https://xdebug.org/wizard
            ;
            ; Uncomment the next line to enable the xdebug extension.
            ; zend_extension=xdebug

            xdebug.log="[GIT_ROOT_DIR_SLASH]/storage/logs/php-xdebug.log"
            xdebug.output_dir="[GIT_ROOT_DIR_SLASH]/storage/logs"

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
