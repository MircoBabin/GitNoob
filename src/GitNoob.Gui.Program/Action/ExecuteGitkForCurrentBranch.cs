using System.Collections.Generic;
using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public class ExecuteGitkForCurrentBranch : Action, IAction
    {
        public ExecuteGitkForCurrentBranch(StepsExecutor.StepConfig Config) : base(Config) { }

        private static string _cacheExecutable = null;
        public static string GetExecutable()
        {
            if (_cacheExecutable == null)
            {
                _cacheExecutable = Utils.FileUtils.FindExePath("gitk.exe");
            }

            return _cacheExecutable;
        }

        public Icon icon()
        {
            return Utils.Resources.getIcon("gitk");
        }

        public void execute()
        {
            var executor = new StepsExecutor.StepsExecutor(stepConfig, new List<StepsExecutor.IExecutableByStepsExecutor>()
            {
                new Step.StartGitkForCurrentBranch(GetExecutable()),
            }, StepsExecutor.StepsExecutor.LockFrontend.No);
            executor.execute();
        }
    }
}
