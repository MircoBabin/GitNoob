using System;

namespace GitNoob.Gui.Visualizer
{
    public interface IVisualizerProgram
    {
        void visualizerSet(IVisualizer visualizer);
        void visualizerExit();
        void visualizerReady(GitResult.StatusResult status);
        void visualizerShowException(Exception ex);

        GitResult.StatusResult visualizerRetrieveStatus();

        string visualizerProjectName();
        string visualizerProjectWorkingDirectoryIconFilename();
        string visualizerProjectWorkingDirectoryName();
        string visualizerProjectWorkingDirectoryImageFilename();
        string visualizerProjectWorkingDirectoryImageBackgroundHtmlColor();
        string visualizerProjectWorkingDirectoryMainBranch();
        string visualizerProjectWorkingDirectoryPath();
        string visualizerProjectWorkingDirectoryProjectTypeName();
        bool visualizerProjectWorkingDirectoryHasGitNoobRemoteUrl();
        bool visualizerProjectWorkingDirectoryTouchTimestampsBeforeMerge();

        IViusalizerAction visualizerChangeBranch();
        IViusalizerAction visualizerStartBrowser();
        IViusalizerAction visualizerStartExplorer();
        IViusalizerAction visualizerStartWorkspace();
        IViusalizerAction visualizerStartDosPromptAsUser();
        IViusalizerAction visualizerStartDosPromptAsAdministrator();
        IViusalizerAction visualizerStartGitGui();
        IViusalizerAction visualizerStartGitkForCurrentBranch();
        void visualizerStartGitkForOneFile(string filename);
        IViusalizerAction visualizerStartGitkAll();
        IViusalizerAction visualizerCherryPick();
        IViusalizerAction visualizerGetLatest();
        IViusalizerAction visualizerMerge();
        IViusalizerAction visualizerDeleteAllChanges();
        IViusalizerAction visualizerGitRepairOptions();

        IViusalizerAction visualizerStartExploreLogFiles();
        IViusalizerAction visualizerOpenConfigFiles();
        IViusalizerAction visualizerClearCache();
        IViusalizerAction visualizerDeleteLogFiles();

        IViusalizerAction visualizerStartSmtpServer();
        IViusalizerAction visualizerStartFiddler();
        IViusalizerAction visualizerStartNgrok();
    }
}
