using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action
{
    public enum IVisualizerMessageType { input, options };

    public interface IVisualizerMessage
    {
        IVisualizerMessageType VisualizerMessageType { get; }
        VisualizerMessageWithLinks VisualizerMessageText { get; }
        List<VisualizerMessageButton> VisualizerMessageButtons { get; }
    }
}
