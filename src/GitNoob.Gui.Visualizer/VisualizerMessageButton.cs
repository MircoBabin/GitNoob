using System.Collections.Generic;

namespace GitNoob.Gui.Visualizer
{
    public class VisualizerMessageButton
    {
        public string text { get; set; }
        public System.Action<VisualizerInput> onClicked { get; set; }
        public List<VisualizerMessageSubButton> subButtons { get; set; }

        public VisualizerMessageButton(string text, System.Action<VisualizerInput> onClicked)
        {
            this.text = text;
            this.onClicked = onClicked;
            this.subButtons = new List<VisualizerMessageSubButton>();
        }

        public VisualizerMessageButton(string text, System.Action<VisualizerInput> onClicked, List<VisualizerMessageSubButton> subButtons)
        {
            this.text = text;
            this.onClicked = onClicked;
            this.subButtons = new List<VisualizerMessageSubButton>();
            if (subButtons != null) this.subButtons.AddRange(subButtons);
        }
    }
}
