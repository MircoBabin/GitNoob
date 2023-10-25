using GitNoob.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace GitNoob.Gui.Program
{
    public class ProgramWorkingDirectory : Visualizer.IVisualizerProgram
    {
        public Visualizer.IVisualizer Visualizer { get; private set; }
        public Config.Project Project { get; }
        public Config.WorkingDirectory ProjectWorkingDirectory { get; }
        public Config.IExecutor Executor { get; }

        public Git.GitWorkingDirectory Git { get; }

        public GitNoob.Utils.ConfigFileTemplate.PhpIni PhpIni { get; }
        public GitNoob.Utils.ConfigFileTemplate.ApacheConf ApacheConf { get; }

        private Action.ChangeBranch _changeBranch;
        private Action.StartBrowser _startBrowser;
        private Action.StartExplorer _startExplorer;
        private Action.StartWorkspace _startWorkspace;
        private Action.StartDosPromptAsUser _startDosPromptAsUser;
        private Action.StartDosPromptAsAdministrator _startDosPromptAsAdministrator;
        private Action.StartGitGui _startGitGui;
        private Action.StartGitkForCurrentBranch _startGitkForCurrentBranch;
        private Action.StartGitk _startGitk;
        private Action.StartGitkAll _startGitkAll;
        private Action.CherryPick _cherryPick;
        private Action.GetLatest _getLatest;
        private Action.Merge _merge;
        private Action.DeleteAllChanges _deleteAllChanges;
        private Action.GitRepairOptions _gitRepairOptions;

        private Action.StartExploreLogfiles _startExploreLogfiles;
        private Action.OpenConfigfiles _openConfigfiles;
        private Action.ClearCache _clearCache;
        private Action.DeleteLogfiles _deleteLogFiles;

        private Action.StartSmtpServer _startSmtpServer;
        private Action.StartFiddler _startFiddler;
        private Action.StartNgrok _startNgrok;

        public ProgramWorkingDirectory(Config.Project project, Config.WorkingDirectory workingdirectory)
        {
            if (String.IsNullOrEmpty(GitNoob.Git.GitWorkingDirectory.getGitExecutable()))
            {
                string gitExecutable = null;
                try
                {
                    gitExecutable = FileUtils.FindExePath("git.exe");
                }
                catch { }

                GitNoob.Git.GitWorkingDirectory.setGitExecutable(gitExecutable);
            }
            Visualizer = null;
            Project = project;
            ProjectWorkingDirectory = workingdirectory;

            Git = new Git.GitWorkingDirectory(ProjectWorkingDirectory);
            PhpIni = new GitNoob.Utils.ConfigFileTemplate.PhpIni(Project, ProjectWorkingDirectory);
            ApacheConf = new GitNoob.Utils.ConfigFileTemplate.ApacheConf(Project, ProjectWorkingDirectory, PhpIni);

            Executor = new BatFile(
                "iexecutor",
                BatFile.RunAsType.runAsInvoker, BatFile.WindowType.hideWindow,
                "ProjectType - Executor",
                Project, ProjectWorkingDirectory,
                PhpIni);

            _changeBranch = new Action.ChangeBranch(this);
            _startBrowser = new Action.StartBrowser(this);
            _startExplorer = new Action.StartExplorer(this);
            _startWorkspace = new Action.StartWorkspace(this);
            _startDosPromptAsUser = new Action.StartDosPromptAsUser(this);
            _startDosPromptAsAdministrator = new Action.StartDosPromptAsAdministrator(this);
            _startGitGui = new Action.StartGitGui(this);
            _startGitkForCurrentBranch = new Action.StartGitkForCurrentBranch(this);
            _startGitk = new Action.StartGitk(this);
            _startGitkAll = new Action.StartGitkAll(this);
            _cherryPick = new Action.CherryPick(this);
            _getLatest = new Action.GetLatest(this);
            _merge = new Action.Merge(this);
            _deleteAllChanges = new Action.DeleteAllChanges(this);
            _gitRepairOptions = new Action.GitRepairOptions(this);

            _startExploreLogfiles = new Action.StartExploreLogfiles(this);
            _openConfigfiles = new Action.OpenConfigfiles(this);
            _clearCache = new Action.ClearCache(this);
            _deleteLogFiles = new Action.DeleteLogfiles(this);

            _startSmtpServer = new Action.StartSmtpServer(this);
            _startFiddler = new Action.StartFiddler(this);
            _startNgrok = new Action.StartNgrok(this);
        }


        public void visualizerSet(Visualizer.IVisualizer visualizer)
        {
            Visualizer = visualizer;
        }

        public void visualizerExit()
        {
            Visualizer = null;

            if (ProjectWorkingDirectory.Git.ClearCommitNameAndEmailOnExit.Value)
            {
                Git.ClearCommitter();
            }
        }

        public void visualizerReady(GitResult.StatusResult status)
        {
            var ready = new Action.VisualizerReady(this);
            ready.execute(status);
        }

        public GitResult.StatusResult visualizerRetrieveStatus()
        {
            return Git.RetrieveStatus();
        }

        public string visualizerProjectName()
        {
            return Project.Name;
        }

        public string visualizerProjectWorkingDirectoryIconFilename()
        {
            string filename = ProjectWorkingDirectory.IconFilename.ToString();
            if (File.Exists(filename))
            {
                return filename;
            }

            filename = Project.IconFilename.ToString();
            if (File.Exists(filename))
            {
                return filename;
            }

            return null;
        }

        public string visualizerProjectWorkingDirectoryName()
        {
            return ProjectWorkingDirectory.Name;
        }

        public string visualizerProjectWorkingDirectoryImageFilename()
        {
            return ProjectWorkingDirectory.ImageFilename.ToString();
        }

        public string visualizerProjectWorkingDirectoryImageBackgroundHtmlColor()
        {
            return ProjectWorkingDirectory.ImageBackgroundColor;
        }

        public string visualizerProjectWorkingDirectoryMainBranch()
        {
            return ProjectWorkingDirectory.Git.MainBranch;
        }

        public string visualizerProjectWorkingDirectoryPath()
        {
            return ProjectWorkingDirectory.Path.ToString();
        }

        public bool visualizerProjectWorkingDirectoryTouchTimestampsBeforeMerge()
        {
            return ProjectWorkingDirectory.Git.TouchTimestampOfCommitsBeforeMerge.Value;
        }

        public Visualizer.IViusalizerAction visualizerChangeBranch()
        {
            return _changeBranch;
        }

        public Visualizer.IViusalizerAction visualizerStartBrowser()
        {
            return _startBrowser;
        }

        public Visualizer.IViusalizerAction visualizerStartExplorer()
        {
            return _startExplorer;
        }

        public Visualizer.IViusalizerAction visualizerStartWorkspace()
        {
            return _startWorkspace;
        }

        public Visualizer.IViusalizerAction visualizerStartDosPromptAsUser()
        {
            return _startDosPromptAsUser;
        }

        public Visualizer.IViusalizerAction visualizerStartDosPromptAsAdministrator()
        {
            return _startDosPromptAsAdministrator;
        }

        public Visualizer.IViusalizerAction visualizerStartGitGui()
        {
            return _startGitGui;
        }

        public Visualizer.IViusalizerAction visualizerStartGitkForCurrentBranch()
        {
            return _startGitkForCurrentBranch;
        }

        public void visualizerStartGitkForOneFile(string filename)
        {
            _startGitk.executeGitk(
                new List<string>() { "HEAD", ProjectWorkingDirectory.Git.MainBranch }, 
                null, 
                new List<string>() { filename });
        }

        public Visualizer.IViusalizerAction visualizerStartGitkAll()
        {
            return _startGitkAll;
        }

        public Visualizer.IViusalizerAction visualizerCherryPick()
        {
            return _cherryPick;
        }

        public Visualizer.IViusalizerAction visualizerGetLatest()
        {
            return _getLatest;
        }

        public Visualizer.IViusalizerAction visualizerMerge()
        {
            return _merge;
        }

        public Visualizer.IViusalizerAction visualizerDeleteAllChanges()
        {
            return _deleteAllChanges;
        }

        public Visualizer.IViusalizerAction visualizerGitRepairOptions()
        {
            return _gitRepairOptions;
        }

        public Visualizer.IViusalizerAction visualizerStartExploreLogFiles()
        {
            return _startExploreLogfiles;
        }

        public Visualizer.IViusalizerAction visualizerOpenConfigFiles()
        {
            return _openConfigfiles;
        }

        public Visualizer.IViusalizerAction visualizerClearCache()
        {
            return _clearCache;
        }

        public Visualizer.IViusalizerAction visualizerDeleteLogFiles()
        {
            return _deleteLogFiles;
        }

        public Visualizer.IViusalizerAction visualizerStartSmtpServer()
        {
            return _startSmtpServer;
        }

        public Visualizer.IViusalizerAction visualizerStartFiddler()
        {
            return _startFiddler;
        }

        public Visualizer.IViusalizerAction visualizerStartNgrok()
        {
            return _startNgrok;
        }
    }
}
