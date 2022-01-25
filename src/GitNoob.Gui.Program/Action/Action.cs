namespace GitNoob.Gui.Program.Action
{
    public class Action
    {
        protected StepsExecutor.StepConfig stepConfig;

        public Action(StepsExecutor.StepConfig Config)
        {
            stepConfig = Config;
        }
    }
}
