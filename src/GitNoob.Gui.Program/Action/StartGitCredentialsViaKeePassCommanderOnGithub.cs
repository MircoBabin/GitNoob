using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public class StartGitCredentialsViaKeePassCommanderOnGithub : Action
    {
        public StartGitCredentialsViaKeePassCommanderOnGithub(ProgramWorkingDirectory Config) : base(Config) { }

        public override bool isStartable()
        {
            return true;
        }

        public override Icon icon()
        {
            return null;
        }

        public override void execute()
        {
            Utils.BatFile.StartWebBrowser(config.visualizerShowException, "https://github.com/MircoBabin/GitCredentialsViaKeePassCommander", 
                config.Project, config.ProjectWorkingDirectory, config.PhpIni);
        }
    }
}
