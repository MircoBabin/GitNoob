namespace GitNoob.GitResult
{
    public class CreateUndeletionTagResult : BaseGitDisasterResult
    {
        public bool Created { get; set; }

        public bool ErrorCreatingSafetyTag { get; set; }

        public CreateUndeletionTagResult()
        {
            Created = false;

            ErrorCreatingSafetyTag = false;
        }
    }
}
