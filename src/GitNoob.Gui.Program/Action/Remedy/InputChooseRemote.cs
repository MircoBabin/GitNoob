using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class InputChooseRemote : Remedy
    {
        public InputChooseRemote(Step.Step Step, MessageWithLinks Message,
            IEnumerable<Git.GitRemote> remotes, string CancelText, System.Action<string> OnSelectedRemoteAction,
            string newRemoteText, System.Action OnCreateRemoteAction) :
            base(Step, ref Message)
        {
            VisualizerMessageButtons = new Dictionary<string, System.Action<MessageInput>>();
            if (!string.IsNullOrWhiteSpace(CancelText))
            {
                VisualizerMessageButtons.Add(CancelText, (input) => { Cancel(); });
            }

            foreach (var remote in remotes)
            {
                VisualizerMessageButtons.Add(remote.RemoteName + " - " + remote.Url, (input) => {
                    OnSelectedRemoteAction(remote.RemoteName);
                    Done();
                });
            }

            if (!string.IsNullOrWhiteSpace(newRemoteText))
            {
                VisualizerMessageButtons.Add(newRemoteText, (input) => {
                    OnCreateRemoteAction();
                    Done();
                });
            }
        }
    }
}
