using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Step
{
    public class TouchTimestampOfCommitsBeforeMerge : Step
    {
        public TouchTimestampOfCommitsBeforeMerge() : base() { }

        protected override bool run()
        {
            if (!StepsExecutor.Config.ProjectWorkingDirectory.Git.TouchTimestampOfCommitsBeforeMerge.Value)
                return true;

            var step = new TouchTimestampsOnCurrentBranch(DateTime.Now);
            StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });
            return true;
        }
    }
}
