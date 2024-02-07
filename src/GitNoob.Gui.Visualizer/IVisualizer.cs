using System;

namespace GitNoob.Gui.Visualizer
{
    public interface IVisualizer
    {
        void lockFrontend();
        bool isFrontendLocked();
        void unlockFrontend();

        void showException(Exception ex);
        void busyMessage(string message);
        void message(IVisualizerMessage message);

        void copyToClipboard(string text);

        void notifyCurrentBranchChanged(string branchname);
    }
}
