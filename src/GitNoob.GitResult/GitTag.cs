namespace GitNoob.GitResult
{
    public class GitTag
    {
        public enum TagType { Annotated, LightWeight }

        public string FullName { get; set; }
        public string ShortName { get; set; }
        public TagType Type { get; set; }
        public string PointingToCommitId { get; set; }
        public string Message { get; set; }

        public GitTag(string FullName, string ShortName, TagType Type, string Commit, string Message)
        {
            this.FullName = FullName;
            this.ShortName = ShortName;
            this.Type = Type;
            this.PointingToCommitId = Commit;
            this.Message = Message;
        }
    }
}
