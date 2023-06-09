using GitNoob.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace GitNoob
{
    internal class Bootstrapper : Gui.Visualizer.IVisualizerBootstrapper
    {
        public Process me { get; }
        public string executableFileName { get; }
        public string programPath { get; }
        public string rootConfigurationFilename { get; set; }
        public List<Config.IConfig> configs { get; private set; }
        public string licenseText { get; private set; }

        public Bootstrapper()
        {
            me = Process.GetCurrentProcess();
            executableFileName = me.Modules[0].FileName;
            programPath = Path.GetFullPath(Path.GetDirectoryName(executableFileName));
            rootConfigurationFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "GitNoob", "GitNoob.ini");
            licenseText = String.Empty;
        }

        private void LoadProjectTypes()
        {
            Config.Loader.ProjectTypeLoader.LoadProjectTypesAssembly(Path.Combine(programPath, "GitNoob.ProjectTypes.dll"));
        }

        private void LoadConfigs()
        {
            configs = new List<Config.IConfig>();

            configs.Add(new Config.Loader.RootIniFileLoader(rootConfigurationFilename, programPath));

            var ProjectsCount = 0;
            foreach (var config in configs)
            {
                foreach (var project in config.GetProjects())
                {
                    ProjectsCount++;
                }
            }

            if (ProjectsCount == 0)
                throw new Exception("There are no projects configured in the root configuration file " + rootConfigurationFilename + ". ");

            CheckConfigs(configs);
        }

        private static void CheckConfigs(List<Config.IConfig> configs)
        {
            Dictionary<string, string> workingDirectoryPaths = new Dictionary<string, string>();
            foreach (var config in configs)
            {
                foreach (var project in config.GetProjects())
                {
                    foreach (var item in project.WorkingDirectories)
                    {
                        var wd = item.Value;
                        var path = wd.Path.ToString().ToLowerInvariant();

                        var name = "Project \"" + project.Name + "\", working directory \"" + wd.Name + "\".";

                        if (workingDirectoryPaths.ContainsKey(path))
                        {
                            string message = "The directory \"" + path + "\" is used in multiple project working directories. This is not allowed." + Environment.NewLine +
                                Environment.NewLine +
                                workingDirectoryPaths[path] + Environment.NewLine +
                                name + Environment.NewLine +
                                Environment.NewLine +
                                "When having 2 mainbranches e.g. \"development\" and \"release\", create 2 directories MyProject-Development and MyProject-Release with their own main branch. The project is then stored 2 times on disk in 2 different directories." + Environment.NewLine;

                            throw new Exception(message);
                        }

                        workingDirectoryPaths.Add(path, name);
                    }
                }
            }
        }

        private void LoadResources()
        {
            Resources.setIcon("error", Properties.Resources.error);

            Resources.setIcon("git gui", Properties.Resources.breeze_icons_5_81_0_git_gui);
            Resources.setIcon("gitk", Properties.Resources.list_symbol_of_three_items_with_dots_icon_icons_com_72994_modified);
            Resources.setIcon("get latest", Properties.Resources.download);
            Resources.setIcon("merge", Properties.Resources.merge);
            Resources.setIcon("delete all changes", Properties.Resources.delete_all_changes);
            Resources.setIcon("git repair", Properties.Resources.wrench_951);

            Resources.setIcon("clear cache", Properties.Resources.clear_cache);
            Resources.setIcon("ngrok", Properties.Resources.ngrok_black);
            Resources.setIcon("open logfiles", Properties.Resources.log_file);
            Resources.setIcon("delete logfiles", Properties.Resources.log_file_delete);
            Resources.setIcon("open configfiles", Properties.Resources.nullset);
        }

        private void LoadLicenseText()
        {
            try
            {
                var asm = Assembly.GetExecutingAssembly();
                var names = asm.GetManifestResourceNames();

                using (var stream = asm.GetManifestResourceStream("GitNoob.LICENSE.md"))
                {
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        licenseText = reader.ReadToEnd();
                    }

                }
            }
            catch { }
        }

        public void Run()
        {
            LoadProjectTypes();
            LoadConfigs();
            LoadResources();
            LoadLicenseText();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(CreateMainForm());
        }

        private Form CreateMainForm()
        {
            return new Gui.Forms.ChooseProjectForm(this, configs, programPath, licenseText);
        }

        public Gui.Visualizer.IVisualizerProgram CreateIVisualizerProgram(Config.Project Project, Config.WorkingDirectory WorkingDirectory)
        {
            return new Gui.Program.ProgramWorkingDirectory(Project, WorkingDirectory);
        }
    }
}
