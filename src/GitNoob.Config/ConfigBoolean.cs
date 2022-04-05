namespace GitNoob.Config
{
    public class ConfigBoolean
    {
        public bool Value { get; private set; }

        public ConfigBoolean(bool value)
        {
            Value = value;
        }

        public void useWorkingDirectory(WorkingDirectory WorkingDirectory)
        {
        }

        public void CopyFrom(ConfigBoolean other)
        {
            Value = other.Value;
        }
    }
}
