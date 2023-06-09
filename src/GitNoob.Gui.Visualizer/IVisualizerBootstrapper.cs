namespace GitNoob.Gui.Visualizer
{
    public interface IVisualizerBootstrapper
    {
        IVisualizerProgram CreateIVisualizerProgram(Config.Project Project, Config.WorkingDirectory WorkingDirectory);
    }
}
