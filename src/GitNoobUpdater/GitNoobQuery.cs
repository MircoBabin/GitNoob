using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace GitNoobUpdater
{
    public static class GitNoobQuery
    {
        private static void RunGitNoobExe(string commandline)
        {
            Process me = Process.GetCurrentProcess();
            string executableFileName = me.Modules[0].FileName;
            string programPath = Path.GetFullPath(Path.GetDirectoryName(executableFileName));

            string exe = Path.Combine(programPath, "GitNoob.exe");
            if (!File.Exists(exe)) throw new Exception("GitNoobUpdater.exe should be run in the same directory as GitNoob.exe! File does not exist: " + exe);

            var info = new ProcessStartInfo
            {
                FileName = exe,
                Arguments = commandline,

                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
            };

            var p = Process.Start(info);
            p.WaitForExit();
        }

        public static GitNoobVersion Version()
        {
            GitNoobVersion result;

            string filename = Path.GetTempFileName();
            try
            {
                RunGitNoobExe("\"--version=" + filename + "\"");

                result = new GitNoobVersion(File.ReadAllText(filename, System.Text.Encoding.UTF8).Trim());
            }
            finally
            {
                try { File.Delete(filename); } catch { }
            }

            return result;
        }

        public static List<string> InstallationFilenames()
        {
            string filename = Path.GetTempFileName();
            try
            {
                RunGitNoobExe("\"--installationFilenames=" + filename + "\"");
            }
            finally
            {
                try { File.Delete(filename); } catch { }
            }

            return null;
        }
    }
}
