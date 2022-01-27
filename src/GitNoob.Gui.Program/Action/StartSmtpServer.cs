using System.Drawing;
using System.IO;

namespace GitNoob.Gui.Program.Action
{
    public class StartSmtpServer : Action, IAction
    {
        public StartSmtpServer(StepsExecutor.StepConfig Config) : base(Config) { }

        private string GetExecutable()
        {
            if (string.IsNullOrWhiteSpace(stepConfig.Config.ProjectWorkingDirectory.SmtpServer.Executable))
            {
                return null;
            }

            return Path.GetFullPath(stepConfig.Config.ProjectWorkingDirectory.SmtpServer.Executable);
        }

        public bool isStartable()
        {
            var smtpserver = GetExecutable();
            if (smtpserver == null) return false;

            if (!File.Exists(smtpserver)) return false;

            return true;
        }

        public Icon icon()
        {
            return Utils.ImageUtils.LoadIconForFile(GetExecutable());
        }

        public void execute()
        {
            if (!isStartable()) return;

            System.Diagnostics.Process.Start(GetExecutable());
        }
    }
}
