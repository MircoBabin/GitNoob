using System;

namespace GitNoob.Gui.Program
{
    public class ProgramWorkingDirectory
    {
        public Config.Project Project { get; }
        public Config.WorkingDirectory ProjectWorkingDirectory { get; }

        public Git.GitWorkingDirectory Git { get; }

        public ConfigFileTemplate.PhpIni PhpIni { get; }
        public ConfigFileTemplate.ApacheConf ApacheConf { get; }

        public ProgramWorkingDirectory(Config.Project project, Config.WorkingDirectory workingdirectory)
        {
            if (String.IsNullOrEmpty(GitNoob.Git.GitWorkingDirectory.getGitExecutable()))
            {
                string gitExecutable = null;
                try
                {
                    gitExecutable = Utils.FileUtils.FindExePath("git.exe");
                }
                catch { }

                GitNoob.Git.GitWorkingDirectory.setGitExecutable(gitExecutable);
            }

            Project = project;
            ProjectWorkingDirectory = workingdirectory;

            Git = new Git.GitWorkingDirectory(ProjectWorkingDirectory);
            PhpIni = new ConfigFileTemplate.PhpIni(Project, ProjectWorkingDirectory);
            ApacheConf = new ConfigFileTemplate.ApacheConf(Project, ProjectWorkingDirectory, PhpIni);
        }
    }
}
