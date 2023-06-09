using System.Drawing;
using System.IO;

namespace GitNoob.Gui.Program.Action
{
    public class StartSmtpServer : Action
    {
        public StartSmtpServer(ProgramWorkingDirectory Config) : base(Config) { }

        private string GetExecutable()
        {
            if (config.ProjectWorkingDirectory.SmtpServer.Executable.isEmpty())
            {
                return null;
            }

            return Path.GetFullPath(config.ProjectWorkingDirectory.SmtpServer.Executable.ToString());
        }

        public override bool isStartable()
        {
            var smtpserver = GetExecutable();
            if (smtpserver == null) return false;

            if (!File.Exists(smtpserver)) return false;

            return true;
        }

        public override Icon icon()
        {
            return Utils.ImageUtils.LoadIconForFile(GetExecutable());
        }

        public override void execute()
        {
            if (!isStartable()) return;

            Utils.BatFile.StartExecutable(GetExecutable(), null,
                config.Project, config.ProjectWorkingDirectory, config.PhpIni);
        }
    }
}
