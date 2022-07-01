using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class MessageLockNotAcquired : Remedy
    {
        public MessageLockNotAcquired(Step.Step Step, VisualizerMessageWithLinks Message, Git.Result.GitLockResult result) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("The branch \"" + result.GitLock.branchName + "\" is logically locked. This indicates a coworker is currently also merging changes into this branch.");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append(Environment.NewLine);
            if (!string.IsNullOrEmpty(result.LockedBy))
            {
                VisualizerMessageText.Append("Locked by: " + result.LockedBy);
                VisualizerMessageText.Append(Environment.NewLine);
            }

            if (result.LockedTime != null && result.LockedTime.Value != null)
            {
                DateTime time = result.LockedTime.Value;
                DateTime today = DateTime.Today;

                VisualizerMessageText.Append("Locked: ");
                if (time.Date == today) VisualizerMessageText.Append("today");
                else if (time.Date.AddDays(1) == today) VisualizerMessageText.Append("yesterday");
                else
                {
                    VisualizerMessageText.Append((today - time.Date).TotalDays + " days ago");
                }

                VisualizerMessageText.Append(" - " + FormatUtils.DateTimeToString(time));
                VisualizerMessageText.Append(Environment.NewLine);
            }

            if (!String.IsNullOrWhiteSpace(result.LockedMessage)) VisualizerMessageText.Append(result.LockedMessage);

            VisualizerMessageButtons =
                new List<VisualizerMessageButton>()
                {
                    new VisualizerMessageButton("Cancel", (input) => {
                        Cancel();
                    }),
                    new VisualizerMessageButton("Retry. Contact the coworker and ask when finished.", (input) => {
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { Step });

                        Done();
                    }),
                    new VisualizerMessageButton("The lock is abondoned. The coworker is not merging changes. Reset the abondoned logical lock and continue.", (input) => {
                        var step = new Step.LockReset(result.GitLock);
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step, Step });

                        Done();
                    }),
                };
        }
    }
}
