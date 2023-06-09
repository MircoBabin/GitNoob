using System;
using System.IO;
using System.Threading;

namespace GitNoob.Gui.Program.Step
{
    public class RenameDirectory : Step
    {
        private string _renameTo;

        public RenameDirectory(string RenameTo) : base()
        {
            _renameTo = RenameTo;
        }

        protected override bool run()
        {
            string busyMessage = "Busy - renaming directory \"" + StepsExecutor.Config.ProjectWorkingDirectory.Path + "\" to \"" + Path.GetFileName(_renameTo) + "\"";
            BusyMessage = busyMessage;

            var path = StepsExecutor.Config.ProjectWorkingDirectory.Path.ToString();
            if (!path.EndsWith("\\")) path = path + "\\";
            string directoryname = new DirectoryInfo(path).Name;

            string newname = _renameTo;
            int tryno = 1;
            while (true)
            {
                try
                {
                    if (!Directory.Exists(StepsExecutor.Config.ProjectWorkingDirectory.Path.ToString())) break;

                    Directory.Move(StepsExecutor.Config.ProjectWorkingDirectory.Path.ToString(), newname);
                    break;
                }
                catch (DirectoryNotFoundException ex)
                {
                    /* 
                     The path specified by sourceDirName is invalid (for example, it is on an unmapped drive). 
                     */

                    //Directory renamed/removed outside GitNoob e.g. in Windows Explorer
                    if (!Directory.Exists(StepsExecutor.Config.ProjectWorkingDirectory.Path.ToString())) break;

                    throw ex;
                }
                catch (IOException ex)
                {
                    /*
                     An attempt was made to move a directory to a different volume. (no)
                     -or-
                     destDirName already exists. See the Note in the Remarks section. (maybe)
                     -or-
                     The sourceDirName and destDirName parameters refer to the same file or directory. (no)
                     -or-
                     The directory or a file within it is being used by another process. (maybe)
                     */

                    //Directory renamed/removed outside GitNoob e.g. in Windows Explorer
                    if (!Directory.Exists(StepsExecutor.Config.ProjectWorkingDirectory.Path.ToString())) break;

                    string details = String.Empty;
                    if (Directory.Exists(newname) || File.Exists(newname))
                    {
                        newname = _renameTo + " (" + tryno + ")";
                    }
                    else
                    {
                        details = "(directory is being used by another process, e.g. Git Gui, Dos prompt, Webserver, GitNoob periodic status check, Virus scanner, ...)";

                    }

                    tryno++;
                    if (tryno > 30) throw ex;

                    Thread.Sleep(1000);
                    BusyMessage = 
                        busyMessage + Environment.NewLine +
                        "Retry " + tryno + " (maximum of 30 retries)" + Environment.NewLine + 
                        details;
                }
            }

            return true;
        }
    }
}
