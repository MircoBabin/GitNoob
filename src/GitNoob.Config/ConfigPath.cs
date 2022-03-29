namespace GitNoob.Config
{
    public class ConfigPath
    {
        private string _orgPath;
        private string _path;

        public ConfigPath(string path)
        {
            _orgPath = path;
            _path = path;
        }

        public void useWorkingDirectory(WorkingDirectory WorkingDirectory)
        {
            if (!string.IsNullOrWhiteSpace(_orgPath))
            {
                _path = _orgPath.Replace("%gitRoot%", WorkingDirectory.Path.ToString());
            }
        }

        public void CopyFrom(ConfigPath other)
        {
            _orgPath = other._orgPath;
            _path = other._path;
        }

        public override string ToString()
        {
            if (isEmpty())
            {
                return string.Empty;
            }

            return _path;
        }

        public bool isEmpty()
        {
            return string.IsNullOrWhiteSpace(_path);
        }
    }
}
