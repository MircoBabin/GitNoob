using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class ClearCache : Step
    {
        public ClearCache() : base() { }

        protected override bool run()
        {
            BusyMessage = "Clear cache";

            var projecttype = StepsExecutor.Config.ProjectWorkingDirectory.ProjectType;
            if (projecttype == null) return true;

            var executor = StepsExecutor.Config.Executor;

            var message = new VisualizerMessageWithLinks("Clearing cache failed.");

            var build = projecttype.ClearCache(executor);
            if (!build.Result)
            {
                FailureRemedy = new Remedy.MessageClearCacheFailed(this, message, build);
                return false;
            }

            return true;
        }
    }
}
