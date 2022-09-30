using System;

namespace GitNoobUpdater
{
    public class ProgramArguments
    {
        public string GitNoobPath;
        public DownloadUrl download;

        public ProgramArguments(string GitNoobPath, DownloadUrl download)
        {
            this.GitNoobPath = GitNoobPath;
            this.download = download;
        }

        public string Serialize()
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(
                GitNoobPath + '\t' + 
                download.downloadUrl + '\t' +
                download.version.Major + '\t' +
                download.version.Minor + '\t' +
                (System.Diagnostics.Debugger.IsAttached ? "debugger" : "") + '\t'));
        }

        public static ProgramArguments Deserialize(string serialized)
        {
            var parts = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(serialized)).Split('\t');
            if (parts.Length < 5) throw new Exception("Invalid ProgramArguments");

            var result = new ProgramArguments(parts[0], 
                new DownloadUrl(parts[1], 
                    new GitNoobVersion(
                        int.Parse(parts[2]), int.Parse(parts[3])
                    )
                )
            );

            if (parts[4] == "debugger") System.Diagnostics.Debugger.Launch();

            return result;
        }
    }
}
