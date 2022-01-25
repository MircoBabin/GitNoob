using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action
{
    public enum IVisualizerMessageType { input, options };

    public interface IVisualizerMessage
    {
        IVisualizerMessageType VisualizerMessageType { get; }
        MessageWithLinks VisualizerMessageText { get; }
        Dictionary<string, System.Action<MessageInput>> VisualizerMessageButtons { get; }
    }
}
