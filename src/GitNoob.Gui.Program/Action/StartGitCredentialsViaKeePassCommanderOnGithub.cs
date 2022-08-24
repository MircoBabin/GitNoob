using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public class StartGitCredentialsViaKeePassCommanderOnGithub : Action, IAction
    {
        public StartGitCredentialsViaKeePassCommanderOnGithub(StepsExecutor.StepConfig Config) : base(Config) { }

        public Icon icon()
        {
            return null;
        }

        public void execute()
        {
            Utils.BatFile.StartWebBrowser("https://github.com/MircoBabin/GitCredentialsViaKeePassCommander", stepConfig.Config.Project, stepConfig.Config.ProjectWorkingDirectory, stepConfig.Config.PhpIni);
        }
    }
}
