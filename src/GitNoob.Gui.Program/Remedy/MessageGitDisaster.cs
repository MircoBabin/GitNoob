using GitNoob.Gui.Visualizer;
using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Remedy
{
    public class MessageGitDisaster : Remedy
    {
        public MessageGitDisaster(Step.Step Step, VisualizerMessageWithLinks Message, GitResult.BaseGitDisasterResult result) :
            base(Step, ref Message)
        {
            VisualizerMessageButtons =
                new List<VisualizerMessageButton>()
                {
                    new VisualizerMessageButton("Cancel", (input) => {
                        Cancel();
                    }),
                };

            if (result.GitDisaster_RebaseInProgress != false)
            {
                if (result.GitDisaster_MergeInProgress != false)
                    VisualizerMessageText.Append("A rebase/merge is in progress.");
                else
                    VisualizerMessageText.Append("A rebase is in progress.");
                return;
            }

            if (result.GitDisaster_MergeInProgress != false)
            {
                VisualizerMessageText.Append("A merge is in progress.");
                return;
            }

            if (result.GitDisaster_CherryPickInProgress != false)
            {
                VisualizerMessageText.Append("A cherry-pick is in progress.");
                return;
            }

            if (result.GitDisaster_RevertInProgress != false)
            {
                VisualizerMessageText.Append("A revert is in progress.");
                return;
            }

            if (result.GitDisaster_DetachedHead != false)
            {
                VisualizerMessageText.Append("There is no current branch, git is in a detached HEAD state.");
                VisualizerMessageText.Append(Environment.NewLine);
                VisualizerMessageText.Append("Change branch to an existing branch.");
                return;
            }

            if (result.GitDisaster_WorkingTreeChanges != false || result.GitDisaster_StagedUncommittedFiles != false)
            {
                VisualizerMessageText.Append("There are ");
                if (result.GitDisaster_WorkingTreeChanges != false)
                {
                    if (result.GitDisaster_StagedUncommittedFiles != false)
                        VisualizerMessageText.Append("working tree changes and staged uncommitted files.");
                    else
                        VisualizerMessageText.Append("working tree changes.");
                }
                else
                {
                    VisualizerMessageText.Append("staged uncommitted files.");
                }
                return;
            }

            if (result.GitDisaster_UnpushedCommitsOnMainBranch != false)
            {
                VisualizerMessageText.Append("The main branch \"" + MainBranch + "\" has unpushed commits.");
                VisualizerMessageText.Append(Environment.NewLine);
                VisualizerMessageText.Append(Environment.NewLine);
                VisualizerMessageText.Append("Resolve this situation by:");
                VisualizerMessageText.Append(Environment.NewLine);
                VisualizerMessageText.Append("- changing branch to \"" + MainBranch + "\".");
                VisualizerMessageText.Append(Environment.NewLine);
                VisualizerMessageText.Append("- and then executing get latest.");


                return;
            }

            throw new Exception("(MessageGitDisaster) Unknown disaster");
        }
    }
}
