namespace GitNoob.Config
{
    public class Editor
    {
        public ConfigFilename WorkspaceFilename { get; private set; }

        public Editor()
        {
            WorkspaceFilename = new ConfigFilename(null);
        }

        public void CopyFrom(Editor other)
        {
            WorkspaceFilename = other.WorkspaceFilename;
        }

        public void useWorkingDirectory(WorkingDirectory WorkingDirectory)
        {
            WorkspaceFilename.useWorkingDirectory(WorkingDirectory);
        }
    }
}
