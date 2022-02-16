using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class InputChooseBranch : Remedy
    {
        public InputChooseBranch(Step.Step Step, MessageWithLinks Message,
            IEnumerable<string> branches, string CancelText, string NewBranchText, string MainBranch, System.Action<string> OnSelectedBranchAction) :
            base(Step, ref Message)
        {
            VisualizerMessageButtons = new Dictionary<string, System.Action<MessageInput>>();
            if (!string.IsNullOrWhiteSpace(CancelText))
            {
                VisualizerMessageButtons.Add(CancelText, (input) => { Cancel(); });
            }

            foreach (var name in branches)
            {
                VisualizerMessageButtons.Add(name, (input) => {
                    OnSelectedBranchAction(name);
                    Done();
                });
            }

            if (!string.IsNullOrWhiteSpace(NewBranchText))
            {
                VisualizerMessageButtons.Add(NewBranchText, (input) => {
                    var remedy = new InputNewBranchName(Step, new MessageWithLinks("Create new branch."), NewBranchText, (NewBranchName) =>
                    {
                        var step = new Step.CreateBranchOntoMainBranch(NewBranchName);
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });
                    });

                    StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor> { remedy });
                    Done();
                });
            }
        }
    }
}
