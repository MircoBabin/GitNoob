using System.IO;
using System.Text;

namespace GitNoob.Config
{
    public class ConfigFilename
    {
        private string _orgFilename;
        private string _filename;

        public ConfigFilename(string filename)
        {
            _orgFilename = filename;
            _filename = filename;
        }

        public void useWorkingDirectory(WorkingDirectory WorkingDirectory)
        {
            if (!string.IsNullOrWhiteSpace(_orgFilename))
            {
                _filename = _orgFilename.Replace("%gitRoot%", WorkingDirectory.Path.ToString());
            }
        }

        public void CopyFrom(ConfigFilename other)
        {
            _orgFilename = other._orgFilename;
            _filename = other._filename;
        }

        public override string ToString()
        {
            if (isEmpty())
            {
                return string.Empty;
            }

            return _filename;
        }

        public bool isEmpty()
        {
            return string.IsNullOrWhiteSpace(_filename);
        }

        public string ReadAllText()
        {
            if (isEmpty())
            {
                return string.Empty;
            }

            string filename = this.ToString();
            if (!File.Exists(filename))
            {
                return string.Empty;
            }

            return File.ReadAllText(filename, Encoding.UTF8);
        }
    }
}
