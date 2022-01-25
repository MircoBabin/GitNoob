using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class MessageStagedUncommittedFiles : Remedy
    {
        public MessageStagedUncommittedFiles(Step.Step Step, MessageWithLinks Message) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("There are staged, uncommitted files in the git staging area.");

            VisualizerMessageButtons =
                new Dictionary<string, System.Action<MessageInput>>()
                {
                    { "Cancel", (input) => {
                        Cancel();
                    } },
                    { "Start Git Gui and manually commit or unstage the staged files", (input) => {
                        StepsExecutor.StartGitGui();
                        Cancel();
                    } }
                };
        }
    }
}
