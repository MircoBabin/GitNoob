using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class InputConfirmGitNoobTemporaryCommits : Remedy
    {
        private bool isSure(string value)
        {
            value = value.Trim().ToLowerInvariant();
            return (value == "sure" || value == "\"sure\"");
        }

        public InputConfirmGitNoobTemporaryCommits(Step.Step Step, MessageWithLinks Message, uint NumberOfCommits) :
            base(Step, ref Message)
        {
            if (NumberOfCommits == 1)
                VisualizerMessageText.Append("There is 1 GitNoob Temporary Commit.");
            else
                VisualizerMessageText.Append("There are " + NumberOfCommits + " GitNoob Temporary Commits.");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("A GitNoob Temporary Commit is created when changing branch with changes in the working directory.");
            VisualizerMessageText.Append(" The changes are committed into a temporary commit. When later changing back to this branch, the temporary commit is unpacked and removed.");
            VisualizerMessageText.Append(" Having a GitNoob Temporary Commit means the branch has changed outside of GitNoob via command \"git checkout ...\".");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("The changes inside a GitNoob Temporary Commit are unlikely to be a complete (nice) set of changes. And normally such a commit is unwanted in the commits history.");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("Confirm this action by typing in \"sure\".");

            VisualizerMessageType = IVisualizerMessageType.input;
            VisualizerMessageButtons = 
                new Dictionary<string, System.Action<MessageInput>>()
                {
                    { "Cancel", (input) => {
                        Cancel();
                    } },
                    { "Continue with having GitNoob Temporary Commits." + Environment.NewLine +
                      "I'm sure and have typed in \"sure\"." + Environment.NewLine, (input) => {
                        if (!isSure(input.inputValue)) return;

                        Done();
                    } },
                };
        }
    }
}
