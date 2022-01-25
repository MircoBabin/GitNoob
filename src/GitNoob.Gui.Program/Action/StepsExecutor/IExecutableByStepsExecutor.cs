namespace GitNoob.Gui.Program.Action.StepsExecutor
{
    public interface IExecutableByStepsExecutor
    {
        StepsExecutor StepsExecutor { get; set; }
        Remedy.Remedy FailureRemedy { get; set; }

        bool execute();
    }
}
