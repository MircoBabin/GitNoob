using System;

namespace GitNoob.Git.Command
{
    public class GetVersionOfGit
    {
        public string fullversion { get; private set; }
        public int? major { get; private set; }
        public int? minor { get; private set; }
        public int? patch { get; private set; }

        private ExecutorGit _executor;

        public GetVersionOfGit(string gitExecutable)
        {
            fullversion = null;
            _executor = new ExecutorGit(gitExecutable, null, "--version");
        }

        public void WaitFor()
        {
            _executor.WaitFor();

            fullversion = _executor.Output.Trim();

            //find first number of version "git version 2.31.1.windows.1"
            int p = 0;
            while (p < fullversion.Length)
            {
                var ch = fullversion[p];
                if (ch >= '0' && ch <= '9') break;

                p++;
            }
            string[] parts;
            if (p >= fullversion.Length)
            {
                parts = new string[0];
            } else {
                parts = fullversion.Substring(p).Split('.');
            }

            Int32 value;

            major = 0;
            if (parts.Length >= 1 && Int32.TryParse(parts[0].Trim(), out value)) major = value;

            minor = 0;
            if (parts.Length >= 2 && Int32.TryParse(parts[1].Trim(), out value)) minor = value;

            patch = 0;
            if (parts.Length >= 3 && Int32.TryParse(parts[2].Trim(), out value)) patch = value;
        }
    }
}
