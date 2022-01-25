using System;

namespace GitNoob.Git.Result
{
    public class ChangeCommitterResult
    {
        public bool Changed { get; set; }

        public string CommitFullName { get; set; }
        public string CommitName { get; set; }
        public string CommitEmail { get; set; }

        public bool ErrorChangingName { get; set; }
        public bool ErrorChangingEmail { get; set; }

        public ChangeCommitterResult()
        {
            Changed = false;

            CommitFullName = string.Empty;
            CommitName = string.Empty;
            CommitEmail = string.Empty;

            ErrorChangingName = false;
            ErrorChangingEmail = false;
        }
    }
}
