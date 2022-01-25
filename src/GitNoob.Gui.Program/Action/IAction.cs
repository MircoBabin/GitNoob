using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public interface IAction
    {
        Icon icon();
        void execute();
    }
}
