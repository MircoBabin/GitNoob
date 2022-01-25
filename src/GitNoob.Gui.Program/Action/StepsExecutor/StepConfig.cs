namespace GitNoob.Gui.Program.Action.StepsExecutor
{
    public class StepConfig
    {
        public ProgramWorkingDirectory Config { get; set; }
        public IVisualizer Visualizer { get; set; }
        public Config.IExecutor Executor { get; set; }

        public StepConfig(ProgramWorkingDirectory Config, IVisualizer Visualizer, Config.IExecutor Executor)
        {
            this.Config = Config;
            this.Visualizer = Visualizer;
            this.Executor = Executor;
        }
    }
}
