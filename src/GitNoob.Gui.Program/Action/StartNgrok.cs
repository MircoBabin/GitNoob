using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace GitNoob.Gui.Program.Action
{
    public class StartNgrok : Action
    {
        public StartNgrok(ProgramWorkingDirectory Config) : base(Config) { }

        private string GetExecutable()
        {
            if (config.ProjectWorkingDirectory.Ngrok.NgrokPath.isEmpty())
            {
                return null;
            }

            return Path.Combine(config.ProjectWorkingDirectory.Ngrok.NgrokPath.ToString(), "ngrok.exe");
        }

        public override bool isStartable()
        {
            if (config.ProjectWorkingDirectory.Apache.Port == 0) return false;

            var ngrok = GetExecutable();
            if (ngrok == null) return false;

            if (!File.Exists(ngrok)) return false;

            return true;
        }

        public override Icon icon()
        {
            return Utils.Resources.getIcon("ngrok");
        }

        public override void execute()
        {
            if (!isStartable()) return;

            if (config.Visualizer.isFrontendLocked()) return;

            var ngrokUrl = "http://localhost:" + config.ProjectWorkingDirectory.Ngrok.Port + "/";
            var executor = new StepsExecutor.StepsExecutor(config, new List<StepsExecutor.IExecutableByStepsExecutor>()
            {
                new Step.StartApache(),
                new Step.StartNgrok(GetExecutable(), ngrokUrl),
                new Step.StartBrowser(ngrokUrl),
            });
            executor.execute();
        }
    }
}
