using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public class StartGitCredentialsViaKeePassCommanderOnGithub : IAction
    {
        public Icon icon()
        {
            return null;
        }

        public void execute()
        {
            System.Diagnostics.Process.Start("https://github.com/MircoBabin/GitCredentialsViaKeePassCommander");
        }
    }
}
