using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class MessageLockNotAcquired : Remedy
    {
        public MessageLockNotAcquired(Step.Step Step, MessageWithLinks Message, Git.Result.GitLockResult result) :
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

                string dayname;
                switch (time.DayOfWeek)
                {
                    case DayOfWeek.Monday: dayname = "monday"; break;
                    case DayOfWeek.Tuesday: dayname = "tuesday"; break;
                    case DayOfWeek.Wednesday: dayname = "wednesday"; break;
                    case DayOfWeek.Thursday: dayname = "thursday"; break;
                    case DayOfWeek.Friday: dayname = "friday"; break;
                    case DayOfWeek.Saturday: dayname = "saturday"; break;
                    case DayOfWeek.Sunday: dayname = "sunday"; break;
                    default: dayname = String.Empty; break;
                }

                string monthname;
                switch (time.Month)
                {
                    case 1: monthname = "january"; break;
                    case 2: monthname = "february"; break;
                    case 3: monthname = "march"; break;
                    case 4: monthname = "april"; break;
                    case 5: monthname = "may"; break;
                    case 6: monthname = "june"; break;
                    case 7: monthname = "july"; break;
                    case 8: monthname = "august"; break;
                    case 9: monthname = "september"; break;
                    case 10: monthname = "october"; break;
                    case 11: monthname = "november"; break;
                    case 12: monthname = "december"; break;
                    default: monthname = String.Empty; break;
                }

                VisualizerMessageText.Append("Locked: ");
                if (time.Date == today) VisualizerMessageText.Append("today");
                else if (time.Date.AddDays(1) == today) VisualizerMessageText.Append("yesterday");
                else
                {
                    VisualizerMessageText.Append((today - time.Date).TotalDays + " days ago");
                }

                VisualizerMessageText.Append(" - " + dayname + " " + time.Day + " " + monthname + " " + time.Year.ToString("0000"));
                VisualizerMessageText.Append(" at " + time.Hour.ToString("00") + ":" + time.Minute.ToString("00") + ":" + time.Second.ToString("00") + "h");
                VisualizerMessageText.Append(Environment.NewLine);
            }

            if (!String.IsNullOrWhiteSpace(result.LockedMessage)) VisualizerMessageText.Append(result.LockedMessage);

            VisualizerMessageButtons =
                new Dictionary<string, System.Action<MessageInput>>()
                {
                    { "Cancel", (input) => {
                        Cancel();
                    } },
                    { "Retry. Contact the coworker and ask when finished.", (input) => {
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { Step });

                        Done();
                    } },
                    { "The lock is abondoned. The coworker is not merging changes. Reset the abondoned logical lock and continue.", (input) => {
                        var step = new Step.LockReset(result.GitLock);
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step, Step });

                        Done();
                    } },
                };
        }
    }
}
