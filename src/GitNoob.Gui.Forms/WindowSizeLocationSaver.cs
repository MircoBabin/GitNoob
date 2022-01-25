using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace GitNoob.Gui.Forms
{
    public static class WindowSizeLocationSaver
    {
        /* 
         * In Project Properties, settings page
         *     Add ChooseProjectForm_Position string
         *     
         * In Form.FormClosing handler
         *     //Save window position
         *     Settings.Default.ChooseProjectForm_Position = WindowSizeLocationSaver.Save(this);
         *     Settings.Default.Save();
         *     //End save window position
         *     
         * In Form.Load handler
         *     //Restore window position
         *     WindowSizeLocationSaver.Restore(this, Settings.Default.ChooseProjectForm_Position);
         *     //End restore window position
         */

        private static Encoding encoding = new UTF8Encoding(false);
        private static XmlSerializer serializer = new XmlSerializer(typeof(NativeMethods.WINDOWPLACEMENT));

        //public is needed for (de)serializing
        public static class NativeMethods
        {
            // RECT structure required by WINDOWPLACEMENT structure
            [Serializable]
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int Left;
                public int Top;
                public int Right;
                public int Bottom;

                public RECT(int left, int top, int right, int bottom)
                {
                    this.Left = left;
                    this.Top = top;
                    this.Right = right;
                    this.Bottom = bottom;
                }
            }

            // POINT structure required by WINDOWPLACEMENT structure
            [Serializable]
            [StructLayout(LayoutKind.Sequential)]
            public struct POINT
            {
                public int X;
                public int Y;

                public POINT(int x, int y)
                {
                    this.X = x;
                    this.Y = y;
                }
            }

            // WINDOWPLACEMENT stores the position, size, and state of a window
            [Serializable]
            [StructLayout(LayoutKind.Sequential)]
            public struct WINDOWPLACEMENT
            {
                public int length;
                public int flags;
                public int showCmd;
                public POINT minPosition;
                public POINT maxPosition;
                public RECT normalPosition;
            }

            [DllImport("user32.dll")]
            public static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

            [DllImport("user32.dll")]
            public static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);

            public const int SW_SHOWNORMAL = 1;
            public const int SW_SHOWMINIMIZED = 2;
        }


        public static string Save(Form form)
        {
            try
            {
                IntPtr windowHandle = form.Handle;
                NativeMethods.WINDOWPLACEMENT placement = new NativeMethods.WINDOWPLACEMENT();
                NativeMethods.GetWindowPlacement(windowHandle, out placement);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, encoding))
                    {
                        serializer.Serialize(xmlTextWriter, placement);
                        return encoding.GetString(memoryStream.ToArray());
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public static void Restore(Form form, string saveString)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(saveString))
                {
                    return;
                }

                IntPtr windowHandle = form.Handle;
                NativeMethods.WINDOWPLACEMENT placement;

                using (MemoryStream memoryStream = new MemoryStream(encoding.GetBytes(saveString)))
                {
                    placement = (NativeMethods.WINDOWPLACEMENT)serializer.Deserialize(memoryStream);
                }

                placement.length = Marshal.SizeOf(typeof(NativeMethods.WINDOWPLACEMENT));
                placement.flags = 0;
                placement.showCmd = (placement.showCmd == NativeMethods.SW_SHOWMINIMIZED ? NativeMethods.SW_SHOWNORMAL : placement.showCmd);

                NativeMethods.SetWindowPlacement(windowHandle, ref placement);
            }
            catch { }
        }
    }
}