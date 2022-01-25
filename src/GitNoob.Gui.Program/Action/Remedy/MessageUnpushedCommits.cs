using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class MessageUnpushedCommits : Remedy
    {
        public MessageUnpushedCommits(Step.Step Step, MessageWithLinks Message, string branchName) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("Branch \"" + branchName + "\" has unpushed commits.");

            VisualizerMessageButtons =
                new Dictionary<string, System.Action<MessageInput>>()
                {
                    { "Cancel", (input) => {
                        Cancel();
                    } }
                };
        }
    }
}
