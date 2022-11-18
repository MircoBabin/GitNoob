using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace GitNoob.Config.Loader
{
    internal class IniFileParser
    {
        public string IniFilename { get; private set; }

        private static class NativeMethods
        {
            [DllImport("kernel32", EntryPoint = "WritePrivateProfileStringW", CharSet = CharSet.Unicode)]
            public static extern long WritePrivateProfileString(string section,
                string key, string val, string filePath);

            [DllImport("kernel32", EntryPoint = "GetPrivateProfileStringW", CharSet = CharSet.Unicode)]
            public static extern int GetPrivateProfileString(string section,
                string key, string def, StringBuilder retVal,
                int size, string filePath);

            [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileSectionNamesW", CharSet = CharSet.Unicode)]
            public static extern uint GetPrivateProfileSectionNames(IntPtr lpszReturnBuffer, uint nSize, string filePath);

            [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileSectionW", CharSet = CharSet.Unicode)]
            public static extern uint GetPrivateProfileSection(string section,
                IntPtr lpszReturnBuffer, uint nSize, string filePath);

        }

        public IniFileParser(string IniFilename)
        {
            var f = new FileInfo(IniFilename);
            this.IniFilename = f.FullName;
        }

        public void WriteValue(string Section, string Key, string Value)
        {
            NativeMethods.WritePrivateProfileString(Section, Key, Value, IniFilename);
        }

        public string ReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(4096);
            NativeMethods.GetPrivateProfileString(Section, Key, "", temp, temp.Capacity, IniFilename);
            return temp.ToString();
        }

        public List<KeyValue> GetSectionKeysAndValues(string Section)
        {
            List<KeyValue> result = new List<KeyValue>();
            string lines = String.Empty;

            IntPtr pReturnedString = IntPtr.Zero;
            try
            {
                const uint MAX_BUFFER = 32767;
                pReturnedString = Marshal.AllocCoTaskMem((int)MAX_BUFFER * 2);
                uint charsReturned = NativeMethods.GetPrivateProfileSection(Section, pReturnedString, MAX_BUFFER, IniFilename);
                if (charsReturned > 0)
                {
                    lines = Marshal.PtrToStringUni(pReturnedString, (int)charsReturned).ToString();
                    if (lines.Length > 1)
                    {
                        foreach (var line in lines.Substring(0, lines.Length - 1).Split('\0'))
                        {
                            string key;
                            string value;

                            var p = line.IndexOf('=');
                            switch (p)
                            {
                                case -1:
                                    key = String.Empty;
                                    value = line;
                                    break;

                                case 0:
                                    key = String.Empty;
                                    value = line.Substring(1);
                                    break;

                                default:
                                    key = line.Substring(0, p);
                                    value = line.Substring(p + 1);
                                    break;
                            }

                            result.Add(new KeyValue(key, value));
                        }
                    }
                }
            }
            finally
            {
                if (pReturnedString != IntPtr.Zero) Marshal.FreeCoTaskMem(pReturnedString);
            }

            return result;
        }

        public List<string> GetSectionNames()
        {
            List<string> result = new List<string>();
            string names = String.Empty;

            IntPtr pReturnedString = IntPtr.Zero;
            try
            {
                const uint MAX_BUFFER = 32767;
                pReturnedString = Marshal.AllocCoTaskMem((int)MAX_BUFFER * 2);
                uint charsReturned = NativeMethods.GetPrivateProfileSectionNames(pReturnedString, MAX_BUFFER, IniFilename);
                if (charsReturned > 0)
                {
                    names = Marshal.PtrToStringUni(pReturnedString, (int)charsReturned).ToString();
                    if (names.Length > 1)
                    {
                        foreach (var name in names.Substring(0, names.Length - 1).Split('\0'))
                        {
                            result.Add(name);
                        }
                    }
                }
            }
            finally
            {
                if (pReturnedString != IntPtr.Zero) Marshal.FreeCoTaskMem(pReturnedString);
            }

            return result;
        }
    }
}
