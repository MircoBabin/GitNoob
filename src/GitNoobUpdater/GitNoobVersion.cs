using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitNoobUpdater
{
    public class GitNoobVersion
    {
        public string FullVersion { get; private set; }
        public int Major { get; private set; }
        public int Minor { get; private set; }
        public GitNoobVersion(string Version)
        {
            FullVersion = Version;

            string[] versionparts = FullVersion.Split('.');
            if (versionparts.Length < 2) throw new Exception("Invalid GitNoob version: \"" + FullVersion + "\"");

            Major = Convert.ToInt32(versionparts[0].Trim());
            Minor = Convert.ToInt32(versionparts[1].Trim());
        }

        public GitNoobVersion(int Major, int Minor)
        {
            this.Major = Major;
            this.Minor = Minor;
            this.FullVersion = this.Major + "." + this.Minor;
        }

        public override string ToString()
        {
            return FullVersion;
        }

        public bool isGreaterThan(GitNoobVersion other)
        {
            return (Major > other.Major) || (Major == other.Major && Minor > other.Minor);
        }
    }
}
