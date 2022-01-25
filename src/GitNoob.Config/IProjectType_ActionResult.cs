using System.Text;

namespace GitNoob.Config
{
    public class IProjectType_ActionResult
    {
        public bool Result { get; set; }
        public string Message { get; set; }
        public string StandardOutput { get; set; }
        public string StandardError { get; set; }

        public IProjectType_ActionResult()
        {
            Result = false;
            Message = string.Empty;
            StandardOutput = string.Empty;
            StandardError = string.Empty;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (Result)
            {
                sb.AppendLine("Success.");
            }
            else
            {
                sb.AppendLine("Failure.");
            }
            sb.AppendLine();

            sb.AppendLine(Message.Trim());
            sb.AppendLine();

            sb.AppendLine("[standard output]");
            sb.AppendLine(StandardOutput);
            sb.AppendLine();

            sb.AppendLine("[standard error]");
            sb.AppendLine(StandardError);
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
