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
        private bool _written = false;

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
            if (_written && File.Exists(_fullFilename)) return;

            var extdir = Path.Combine(_projectWorkingDirectory.Php.Path, "ext");
            var contents = Utils.FileUtils.TemplateToContents(_projectWorkingDirectory.Php.PhpIniTemplateContents, _project, _projectWorkingDirectory,
                new Dictionary<string, string>()
                {
                    { "PHP_EXTENSION_DIR", extdir },
                    { "PHP_EXTENSION_DIR_SLASH", extdir.Replace('\\', '/') },
                });

            /*
             * GitNoob assumption
             * 

            ; Directory in which the loadable extensions (modules) reside.
            ; http://php.net/extension-dir
            ; extension_dir = "./"
            ; On windows:
            extension_dir = "[PHP_EXTENSION_DIR_SLASH]"
            */

            File.WriteAllText(_fullFilename, contents, Encoding.UTF8);
            _written = true;
        }
    }
}
