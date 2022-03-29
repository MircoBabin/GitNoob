using System.Collections.Generic;
using System.IO;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class RenameExistingDirectory : Remedy
    {
        public RenameExistingDirectory(Step.Step Step, MessageWithLinks Message) :
            base(Step, ref Message)
        {
            string RenameTo = Utils.FileUtils.DirectoryCopyRenameToDestinationName(StepsExecutor.Config.ProjectWorkingDirectory.Path.ToString());

            VisualizerMessageButtons = 
                new Dictionary<string, System.Action<MessageInput>>()
                {
                    { "Cancel", (input) => {
                        Cancel();
                    } },
                    { "Open Windows Explorer", (input) => {
                        StepsExecutor.StartExplorer();
                    } },
                    { "Rename the directory \"" + StepsExecutor.Config.ProjectWorkingDirectory.Path + "\" to \"" + Path.GetFileName(RenameTo) + "\"", (input) => {
                        var step = new Step.RenameDirectory(RenameTo);
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step, Step });
                        
                        Done();
                    } }
                };
        }
    }
}
