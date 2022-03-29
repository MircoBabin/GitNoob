using System;
using System.Collections.Generic;
using System.Text;

namespace GitNoob.Gui.Program.Action.Step
{
    public class EditFiles : Step
    {
        private static string _cacheExecutable = null;
        private static bool _cacheExecutableMultipleFiles = false;
        private static Tuple<string,bool> GetExecutable()
        {
            if (_cacheExecutable == null)
            {
                if (_cacheExecutable == null)
                {
                    try
                    {
                        _cacheExecutable = Utils.FileUtils.FindExeInProgramFiles("Notepad++\\notepad++.exe");
                        _cacheExecutableMultipleFiles = true;
                    }
                    catch { }
                }

                if (_cacheExecutable == null)
                {
                    _cacheExecutable = Utils.FileUtils.FindExePath("notepad.exe");
                    _cacheExecutableMultipleFiles = false;
                }
            }

            return new Tuple<string, bool>(_cacheExecutable, _cacheExecutableMultipleFiles);
        }

        private IEnumerable<string> _files;
        public EditFiles(IEnumerable<string> files) : base()
        {
            _files = files;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - opening files";

            try
            {
                var setting = GetExecutable();
                string exe = setting.Item1;
                bool multipleFiles = setting.Item2;

                if (multipleFiles)
                {
                    StringBuilder cmdline = new StringBuilder();
                    foreach(var file in _files)
                    {
                        cmdline.Append(" \"");
                        cmdline.Append(file);
                        cmdline.Append("\"");
                    }

                    var info = new System.Diagnostics.ProcessStartInfo
                    {
                        WorkingDirectory = StepsExecutor.Config.ProjectWorkingDirectory.Path.ToString(),
                        FileName = exe,
                        Arguments = cmdline.ToString(),
                        UseShellExecute = false,
                    };
                    System.Diagnostics.Process.Start(info);
                }
                else
                {
                    foreach (var file in _files)
                    {
                        var info = new System.Diagnostics.ProcessStartInfo
                        {
                            WorkingDirectory = StepsExecutor.Config.ProjectWorkingDirectory.Path.ToString(),
                            FileName = exe,
                            Arguments = "\"" + file +"\"",
                            UseShellExecute = false,
                        };
                        System.Diagnostics.Process.Start(info);
                    }

                }
            }
            catch (Exception ex)
            {
                FailureRemedy = new Remedy.MessageException(this, new MessageWithLinks(), ex);
                return false;
            }

            return true;
        }
    }
}
