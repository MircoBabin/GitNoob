using System.Collections.Generic;

namespace GitNoob.Gui.Visualizer
{
    public enum IVisualizerMessageType { input, input2, options };

    public interface IVisualizerMessage
    {
        IVisualizerMessageType VisualizerMessageType { get; }
        VisualizerMessageWithLinks VisualizerMessageText { get; }
        VisualizerMessageWithLinks VisualizerMessageInput2 { get; }
        List<VisualizerMessageButton> VisualizerMessageButtons { get; }
    }
}
