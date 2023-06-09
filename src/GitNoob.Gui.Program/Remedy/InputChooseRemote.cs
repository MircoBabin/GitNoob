using GitNoob.Gui.Visualizer;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Remedy
{
    public class InputChooseRemote : Remedy
    {
        public InputChooseRemote(Step.Step Step, VisualizerMessageWithLinks Message,
            IEnumerable<GitResult.GitRemote> remotes, string CancelText, System.Action<string> OnSelectedRemoteAction,
            string newRemoteText, System.Action OnCreateRemoteAction) :
            base(Step, ref Message)
        {
            VisualizerMessageButtons = new List<VisualizerMessageButton>();
            if (!string.IsNullOrWhiteSpace(CancelText))
            {
                VisualizerMessageButtons.Add(new VisualizerMessageButton(CancelText, (input) => { Cancel(); }));
            }

            foreach (var remote in remotes)
            {
                VisualizerMessageButtons.Add(new VisualizerMessageButton(remote.RemoteName + " - " + remote.Url, (input) => {
                    OnSelectedRemoteAction(remote.RemoteName);
                    Done();
                }));
            }

            if (!string.IsNullOrWhiteSpace(newRemoteText))
            {
                VisualizerMessageButtons.Add(new VisualizerMessageButton(newRemoteText, (input) => {
                    OnCreateRemoteAction();
                    Done();
                }));
            }
        }
    }
}
