using GitNoob.Gui.Visualizer;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Remedy
{
    public class InputChooseBranch : Remedy
    {
        public InputChooseBranch(Step.Step Step, VisualizerMessageWithLinks Message,
            IEnumerable<string> branches, string CancelText, 
            string RenameCurrentBranchText, string CurrentBranch,
            string NewBranchText, string MainBranch,
            string NewBranchOnSpecificCommitText,
            string MoveChangesText,
            string DeleteBranchText, 
            System.Action<string> OnSelectedBranchAction) :
            base(Step, ref Message)
        {
            VisualizerMessageButtons = new List<VisualizerMessageButton>();
            if (!string.IsNullOrWhiteSpace(CancelText))
            {
                VisualizerMessageButtons.Add(new VisualizerMessageButton(CancelText, (input) => { Cancel(); }));
            }

            foreach (var name in branches)
            {
                VisualizerMessageButtons.Add(new VisualizerMessageButton(name, (input) => {
                    OnSelectedBranchAction(name);
                    Done();
                }));
            }

            if (!string.IsNullOrWhiteSpace(RenameCurrentBranchText))
            {
                VisualizerMessageButtons.Add(new VisualizerMessageButton(RenameCurrentBranchText, (input) => {
                    var remedy = new InputNewBranchName(Step, new VisualizerMessageWithLinks("Rename branch."), RenameCurrentBranchText, false, (NewBranchName, OnCommitId) =>
                    {
                        var step = new Step.RenameBranch(CurrentBranch, NewBranchName);
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });
                    });

                    StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor> { remedy });
                    Done();
                }));
            }

            if (!string.IsNullOrWhiteSpace(NewBranchText))
            {
                VisualizerMessageButtons.Add(new VisualizerMessageButton(NewBranchText, (input) => {
                    var remedy = new InputNewBranchName(Step, new VisualizerMessageWithLinks("Create new branch."), NewBranchText, false, (NewBranchName, OnCommitId) =>
                    {
                        var step = new Step.CreateBranchOntoMainBranch(NewBranchName);
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });
                    });

                    StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor> { remedy });
                    Done();
                }));
            }

            if (!string.IsNullOrWhiteSpace(NewBranchOnSpecificCommitText))
            {
                VisualizerMessageButtons.Add(new VisualizerMessageButton(NewBranchOnSpecificCommitText, (input) => {
                    var remedy = new InputNewBranchName(Step, new VisualizerMessageWithLinks("Create new branch on specific commit-id."), NewBranchOnSpecificCommitText, true, (NewBranchName, OnCommitId) =>
                    {
                        var step = new Step.CreateBranchOntoCommitId(NewBranchName, OnCommitId);
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });
                    });

                    StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor> { remedy });
                    Done();
                }));
            }

            if (!string.IsNullOrWhiteSpace(MoveChangesText))
            {
                VisualizerMessageButtons.Add(new VisualizerMessageButton(MoveChangesText, (input) => {
                    var remedy = new InputNewBranchName(Step, new VisualizerMessageWithLinks("Create new branch."), MoveChangesText, false, (NewBranchName, OnCommitId) =>
                    {
                        var step = new Step.MoveCurrentChangesToNewBranch(NewBranchName);
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });
                    });

                    StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor> { remedy });
                    Done();
                }));
            }

            if (!string.IsNullOrWhiteSpace(DeleteBranchText))
            {
                VisualizerMessageButtons.Add(new VisualizerMessageButton(DeleteBranchText, (input) => {
                    var remedy = new InputConfirmDeleteBranch(Step, new VisualizerMessageWithLinks(), CurrentBranch, (inputMsg) =>
                    {
                        var step = new Step.DeleteCurrentBranch(CurrentBranch, inputMsg.inputValue);
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });
                    });
                    StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { remedy });
                    Done();
                }));
            }
        }
    }
}
