namespace GitNoob.Gui.Program.Action
{
    public interface IVisualizer
    {
        void lockFrontend();
        bool isFrontendLocked();
        void unlockFrontend();

        void busyMessage(string message);
        void message(IVisualizerMessage message);

        void copyToClipboard(string text);

        void notifyCurrentBranchChanged(string branchname);
    }
}
