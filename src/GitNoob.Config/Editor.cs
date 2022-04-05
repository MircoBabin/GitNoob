namespace GitNoob.Config
{
    public class Editor
    {
        public ConfigFilename WorkspaceFilename { get; private set; }
        public ConfigBoolean WorkspaceRunAsAdministrator { get; set; }

        public Editor()
        {
            WorkspaceFilename = new ConfigFilename(null);
            WorkspaceRunAsAdministrator = new ConfigBoolean(false);
        }

        public void CopyFrom(Editor other)
        {
            WorkspaceFilename = other.WorkspaceFilename;
            WorkspaceRunAsAdministrator = other.WorkspaceRunAsAdministrator;
        }

        public void useWorkingDirectory(WorkingDirectory WorkingDirectory)
        {
            WorkspaceFilename.useWorkingDirectory(WorkingDirectory);
            WorkspaceRunAsAdministrator.useWorkingDirectory(WorkingDirectory);
        }
    }
}
