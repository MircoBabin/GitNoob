namespace GitNoob.Gui.Program.StepsExecutor
{
    public interface IExecutableByStepsExecutor
    {
        StepsExecutor StepsExecutor { get; set; }
        Remedy.Remedy FailureRemedy { get; set; }

        bool execute();
    }
}
