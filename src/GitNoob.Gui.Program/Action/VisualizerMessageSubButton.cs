using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public class VisualizerMessageSubButton
    {
        public Icon icon { get; set; }
        public string hint { get; set; }
        public System.Action<VisualizerInput> onClicked { get; set; }
    }
}
