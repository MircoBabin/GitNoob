using GitNoob.GitResult;
using System;

namespace GitNoob.Git
{
    public partial class GitWorkingDirectory
    {
        public ChangeCommitterResult ClearCommitter()
        {
            var clear = new Command.Config.ClearCommitter(this);
            clear.WaitFor();

            var commitname = new Command.Config.GetCurrentCommitter(this);
            commitname.WaitFor();

            return new ChangeCommitterResult
            {
                Changed = (String.IsNullOrWhiteSpace(commitname.name) && String.IsNullOrWhiteSpace(commitname.email)),

                CommitFullName = commitname.result,
                CommitName = commitname.name,
                CommitEmail = commitname.email,

                ErrorChangingName = (!String.IsNullOrWhiteSpace(commitname.name)),
                ErrorChangingEmail = (!String.IsNullOrWhiteSpace(commitname.email)),
            };
        }

        public ChangeCommitterResult ChangeCommitter(string toName, string toEmail)
        {
            var set = new Command.Config.SetCommitter(this, toName, toEmail);
            set.WaitFor();

            var commitname = new Command.Config.GetCurrentCommitter(this);
            commitname.WaitFor();

            return new ChangeCommitterResult
            {
                Changed = (commitname.name == toName && commitname.email == toEmail),

                CommitFullName = commitname.result,
                CommitName = commitname.name,
                CommitEmail = commitname.email,

                ErrorChangingName = (commitname.name != toName),
                ErrorChangingEmail = (commitname.email != toEmail),
            };
        }
    }
}
