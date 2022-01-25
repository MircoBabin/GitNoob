using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class InputNewBranchName : Remedy
    {
        public InputNewBranchName(Step.Step Step, MessageWithLinks Message, 
            string MoveButtonText, System.Action<string> OnMoveButtonAction) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("A branch name can only contain A-Z, a-z, 0-9, _ (underscore) and - (dash). And should not start with \"gitlock-\" or \"gitnoob-\".");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("Enter the name of the new branch:");

            VisualizerMessageType = IVisualizerMessageType.input;
            VisualizerMessageButtons = 
                new Dictionary<string, System.Action<MessageInput>>()
                {
                    { "Cancel", (input) => {
                        Cancel();
                    } },
                    { MoveButtonText, (input) => {
                        if (input.inputValue.ToLowerInvariant().StartsWith("gitnoob-")) return;
                        if (input.inputValue.ToLowerInvariant().StartsWith("gitlock-")) return;

                        foreach(var c in input.inputValue)
                        {
                            bool valid =
                               (c >= 'A' && c <= 'Z') ||
                               (c >= 'a' && c <= 'z') ||
                               (c >= '0' && c <= '9') ||
                               (c == '_') ||
                               (c == '-');
                            if (!valid) return;
                        }

                        OnMoveButtonAction(input.inputValue);
                        Done();
                    } }
                };
        }
    }
}
