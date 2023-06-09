using GitNoob.Gui.Visualizer;
using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public abstract class Action : IViusalizerAction
    {
        protected ProgramWorkingDirectory config;

        public Action(ProgramWorkingDirectory Config)
        {
            config = Config;
        }

        public abstract bool isStartable();
        public abstract Icon icon();
        public abstract void execute();

    }
}
