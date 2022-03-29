using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace GitNoob.Gui.Program.Action
{
    public class ExecuteStartNgrok : Action, IAction
    {
        public ExecuteStartNgrok(StepsExecutor.StepConfig Config) : base(Config) { }

        private string GetExecutable()
        {
            if (stepConfig.Config.ProjectWorkingDirectory.Ngrok.NgrokPath.isEmpty())
            {
                return null;
            }

            return Path.Combine(stepConfig.Config.ProjectWorkingDirectory.Ngrok.NgrokPath.ToString(), "ngrok.exe");
        }

        public bool isStartable()
        {
            if (stepConfig.Config.ProjectWorkingDirectory.Apache.Port == 0) return false;

            var ngrok = GetExecutable();
            if (ngrok == null) return false;

            if (!File.Exists(ngrok)) return false;

            return true;
        }

        public Icon icon()
        {
            return Utils.Resources.getIcon("ngrok");
        }

        public void execute()
        {
            if (!isStartable()) return;

            if (stepConfig.Visualizer.isFrontendLocked()) return;

            var ngrokUrl = "http://localhost:" + stepConfig.Config.ProjectWorkingDirectory.Ngrok.Port + "/";
            var executor = new StepsExecutor.StepsExecutor(stepConfig, new List<StepsExecutor.IExecutableByStepsExecutor>()
            {
                new Step.StartApache(),
                new Step.StartNgrok(GetExecutable(), ngrokUrl),
                new Step.StartBrowser(ngrokUrl),
            });
            executor.execute();
        }
    }
}
