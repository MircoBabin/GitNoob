namespace GitNoob.Config
{
    public class IProjectType_Capabilities
    {
        public bool NeedsPhp { get; set; }
        public bool CapableOfClearAndBuildCache { get; set; }

        public IProjectType_Capabilities()
        {
            NeedsPhp = false;
            CapableOfClearAndBuildCache = false;
        }
    }
}
