using System;

namespace GitNoob.GitResult
{
    public class GitReflog
    {
        public enum TagType { Annotated, LightWeight }

        public string CommitId { get; set; }
        public string Selector { get; set; }
        public string Message { get; set; }

        public DateTime? CommitTime { get; set; }
        public string CommitMessage { get; set; }

        public GitReflog(string CommitId, string Selector, string Message, DateTime? CommitTime, string CommitMessage)
        {
            this.CommitId = CommitId;
            this.Selector = Selector;
            this.Message = Message;

            this.CommitTime = CommitTime;
            this.CommitMessage = CommitMessage;
        }
    }
}
