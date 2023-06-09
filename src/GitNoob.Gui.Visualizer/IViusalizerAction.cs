using System.Drawing;

namespace GitNoob.Gui.Visualizer
{
    public interface IViusalizerAction
    {
        bool isStartable();
        Icon icon();
        void execute();
    }
}
