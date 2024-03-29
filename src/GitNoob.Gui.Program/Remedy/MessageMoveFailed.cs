﻿using GitNoob.Gui.Visualizer;
using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Remedy
{
    public class MessageMoveFailed : Remedy
    {
        public MessageMoveFailed(Step.Step Step, VisualizerMessageWithLinks Message, 
            string fromBranch, string toBranch, string currentBranch,
            bool RenameFailed, bool RemoveRemoteFailed) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("Moving changes from branch \"" + fromBranch + "\" to \"" + toBranch + "\" failed.");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("The current branch is now \"" + currentBranch + "\".");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append(Environment.NewLine);
            if (RenameFailed)
            {
                VisualizerMessageText.Append("(Rename failed.)");
            }
            if(RemoveRemoteFailed)
            {
                VisualizerMessageText.Append("(Remove remote after rename failed.)");
            }

            VisualizerMessageButtons =
                new List<VisualizerMessageButton>()
                {
                    new VisualizerMessageButton("Cancel", (input) => {
                        Cancel();
                    }),
                };
        }
    }
}
