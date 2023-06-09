using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class FinishRebaseMerge : Step
    {
        public FinishRebaseMerge() : base() { }

        protected override bool run()
        {
            BusyMessage = "Busy - finish rebase/merge";

            var result = StepsExecutor.Config.Git.RetrieveStatus();

            if (result.Rebasing && result.Merging)
            {
                FailureRemedy = new Remedy.FinishRebaseMerge(this, new VisualizerMessageWithLinks("A rebase and/or merge is in progress. Unable to detect wether a rebase or a merge is busy."), result.Rebasing, result.Merging);
                return false;
            }

            if (result.Rebasing)
            {
                if (result.Conflicts)
                {
                    FailureRemedy = new Remedy.ResolveRebaseConflicts(this, new VisualizerMessageWithLinks("A rebase is in progress."), null, null);
                    return false;
                }

                FailureRemedy = new Remedy.FinishRebaseMerge(this, new VisualizerMessageWithLinks("A rebase is in progress."), result.Rebasing, result.Merging);
                return false;
            }

            if (result.Merging)
            {
                if (result.Conflicts)
                {
                    FailureRemedy = new Remedy.ResolveMergeConflicts(this, new VisualizerMessageWithLinks("A merge is in progress."));
                    return false;
                }

                FailureRemedy = new Remedy.FinishRebaseMerge(this, new VisualizerMessageWithLinks("A merge is in progress."), result.Rebasing, result.Merging);
                return false;
            }

            return true;
        }
    }
}

