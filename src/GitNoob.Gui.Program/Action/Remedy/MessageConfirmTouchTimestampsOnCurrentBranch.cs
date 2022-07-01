﻿using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class MessageConfirmTouchTimestampsOnCurrentBranch : Remedy
    {
        public MessageConfirmTouchTimestampsOnCurrentBranch(Step.Step Step, VisualizerMessageWithLinks Message, string currentBranch) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("Set the author and commit timestamps of all unmerged commits on the current branch \"" + currentBranch + "\"." + Environment.NewLine);

            VisualizerMessageButtons = 
                new List<VisualizerMessageButton>()
                {
                    new VisualizerMessageButton("Cancel", (input) => {
                        Cancel();
                    }),
                    new VisualizerMessageButton("Set author and commit timestamps to the current system time (now).", (input) => {
                        var step = new Step.TouchTimestampsOnCurrentBranch(DateTime.Now);
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });
                        Done();
                    }),
                };
        }
    }
}