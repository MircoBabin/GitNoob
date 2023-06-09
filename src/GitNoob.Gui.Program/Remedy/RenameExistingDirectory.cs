using GitNoob.Gui.Visualizer;
using GitNoob.Utils;
using System.Collections.Generic;
using System.IO;

namespace GitNoob.Gui.Program.Remedy
{
    public class RenameExistingDirectory : Remedy
    {
        public RenameExistingDirectory(Step.Step Step, VisualizerMessageWithLinks Message) :
            base(Step, ref Message)
        {
            string RenameTo = FileUtils.DirectoryCopyRenameToDestinationName(StepsExecutor.Config.ProjectWorkingDirectory.Path.ToString());

            VisualizerMessageButtons = 
                new List<VisualizerMessageButton>()
                {
                    new VisualizerMessageButton("Cancel", (input) => {
                        Cancel();
                    }),
                    new VisualizerMessageButton("Open Windows Explorer", (input) => {
                        StepsExecutor.StartExplorer();
                    }),
                    new VisualizerMessageButton("Rename the directory \"" + StepsExecutor.Config.ProjectWorkingDirectory.Path + "\" to \"" + Path.GetFileName(RenameTo) + "\"", (input) => {
                        var step = new Step.RenameDirectory(RenameTo);
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step, Step });
                        
                        Done();
                    }),
                };
        }
    }
}
