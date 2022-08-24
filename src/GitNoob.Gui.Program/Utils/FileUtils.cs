using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GitNoob.Gui.Program.Utils
{
    public static class FileUtils
    {
        public static string FindExeInProgramFiles(string subpathAndExe)
        {
            subpathAndExe = Environment.ExpandEnvironmentVariables(subpathAndExe);

            try
            {
                string exe = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), subpathAndExe);
                if (File.Exists(exe)) return exe;
            }
            catch { }

            try
            {
                string exe = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), subpathAndExe);
                if (File.Exists(exe)) return exe;
            }
            catch { }

            throw new FileNotFoundException(new FileNotFoundException().Message, subpathAndExe);
        }

        public static string FindExePath(string exe)
        {
            exe = Environment.ExpandEnvironmentVariables(exe);
            if (File.Exists(exe))
                return Path.GetFullPath(exe);

            if (!String.IsNullOrEmpty(Path.GetDirectoryName(exe)))
                throw new FileNotFoundException(new FileNotFoundException().Message, exe);

            string path = Environment.GetEnvironmentVariable("PATH");
            string[] paths = path.Split(new char[1] { Path.PathSeparator });
            foreach (string directory in paths)
            {
                string dir = directory.Trim();
                if (dir.Length > 2 && dir.StartsWith("\""))
                    dir = dir.Substring(1, dir.Length - 2);

                string tryexe = Path.Combine(dir, exe);
                if (!String.IsNullOrEmpty(path) && File.Exists(tryexe))
                    return Path.GetFullPath(tryexe);
            }

            throw new FileNotFoundException(new FileNotFoundException().Message, exe);
        }

        public static string GetIconFilenameFor(string filename)
        {
            var extension = Path.GetExtension(filename).ToLowerInvariant(); //including leading "."
            if (String.IsNullOrEmpty(extension)) return String.Empty;

            if (extension == ".exe")
            {
                return filename;
            }

            if (extension == ".ico")
            {
                return filename;
            }

            //Query from registry HKEY_CLASSES_ROOT\.code-workspace
            string extensionValue = String.Empty;
            using (var key = Registry.ClassesRoot.OpenSubKey(extension, false))
            {
                if (key != null) extensionValue = (string)key.GetValue("");
            }

            //Query from HKEY_CLASSES_ROOT\{extensionValue}\shell\open\command
            string openCommand = String.Empty;
            using (var key = Registry.ClassesRoot.OpenSubKey(extensionValue + "\\shell\\open\\command", false))
            {
                if (key != null) openCommand = ((string)key.GetValue("")).Trim();
            }

            //Split {executable} {commandline}
            string executable = String.Empty;
            if (openCommand.StartsWith("\""))
            {
                executable = openCommand.Substring(1, openCommand.IndexOf('"', 1) - 1);
            }
            else
            {
                executable = openCommand.Substring(0, openCommand.IndexOf(' '));
            }

            return FindExePath(executable);
        }

        public static string TempDirectoryForProject(Config.Project project, Config.WorkingDirectory projectworkingdirectory)
        {
            // Don't use Path.GetTempPath() as files inside may be deleted by the Windows OS at unexpected moments, when GitNoob is still running.
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GitNoob");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (project != null && !string.IsNullOrWhiteSpace(project.Name))
            {
                path = Path.Combine(path, DeriveFilename("project-", project.Name));
            }
            else
            {
                path = Path.Combine(path, "project-noname");
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (projectworkingdirectory != null)
            {
                path = Path.Combine(path, DeriveFilename("directory-", projectworkingdirectory.Name));
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }

            return path;
        }

        public static string TemplateToContents(string template, Config.Project project, Config.WorkingDirectory projectworkingdirectory, Dictionary<string, string> extra)
        {
            StringBuilder result = new StringBuilder(template);

            if (extra != null)
            {
                foreach (var item in extra)
                {
                    result.Replace("[" + item.Key + "]", item.Value);
                }
            }

            return result.ToString();
        }

        public static string DirectoryCopyRenameToDestinationName(string sourceDirName)
        {
            if (sourceDirName.EndsWith("\\")) sourceDirName = sourceDirName.Substring(0, sourceDirName.Length - 1);

            DateTime time = DateTime.Now;
            return sourceDirName + "." + time.ToString("yyyy-MM-dd") + " at " + time.ToString("HH") + "h " + time.ToString("mm");
        }

        public static string DeriveFilename(string prefix, string name)
        {
            var result = new StringBuilder();
            result.Append(prefix);

            for (var i = 0; i < name.Length; i++)
            {
                var ch = name[i];
                switch (ch)
                {
                    case '-':
                    case '_':

                    case 'A':
                    case 'B':
                    case 'C':
                    case 'D':
                    case 'E':
                    case 'F':
                    case 'G':
                    case 'H':
                    case 'I':
                    case 'J':
                    case 'K':
                    case 'L':
                    case 'M':
                    case 'N':
                    case 'O':
                    case 'P':
                    case 'Q':
                    case 'R':
                    case 'S':
                    case 'T':
                    case 'U':
                    case 'V':
                    case 'W':
                    case 'X':
                    case 'Y':
                    case 'Z':

                    case 'a':
                    case 'b':
                    case 'c':
                    case 'd':
                    case 'e':
                    case 'f':
                    case 'g':
                    case 'h':
                    case 'i':
                    case 'j':
                    case 'k':
                    case 'l':
                    case 'm':
                    case 'n':
                    case 'o':
                    case 'p':
                    case 'q':
                    case 'r':
                    case 's':
                    case 't':
                    case 'u':
                    case 'v':
                    case 'w':
                    case 'x':
                    case 'y':
                    case 'z':
                        result.Append(ch);
                        break;
                }
            }

            return result.ToString();
        }

        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
                if (!Directory.Exists(destDirName))
                    throw new DirectoryNotFoundException(
                        "Destination directory does not exist or could not be created: "
                        + destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        public static void DirectoryDeleteFilesExcludingDotGitFiles(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            if (!dir.Exists) return;

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                if (!file.Name.ToLowerInvariant().StartsWith(".git"))
                {
                    try
                    {
                        file.Delete();
                    }
                    catch { }
                }
            }
        }
    }
}
