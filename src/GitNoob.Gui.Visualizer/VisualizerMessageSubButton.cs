using System.Drawing;

namespace GitNoob.Gui.Visualizer
{
    public class VisualizerMessageSubButton
    {
        public Icon icon { get; set; }
        public string hint { get; set; }
        public System.Action<VisualizerInput> onClicked { get; set; }

        public VisualizerMessageSubButton(Icon icon, string hint, System.Action<VisualizerInput> onClicked)
        {
            this.icon = icon;
            this.hint = hint;
            this.onClicked = onClicked;
        }
    }
}
