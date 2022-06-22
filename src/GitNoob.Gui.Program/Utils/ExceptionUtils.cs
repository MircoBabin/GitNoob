using System;
using System.Text;

namespace GitNoob.Gui.Program.Utils
{
    public static class ExceptionUtils
    {
        public static string GetFullExceptionText(Exception ex)
        {
            var text = new StringBuilder();

            while (true)
            {
                text.Append("[");
                text.Append(ex.GetType().Name);
                text.Append("] ");
                text.AppendLine(ex.Message);
                text.AppendLine(ex.StackTrace);

                if (ex.InnerException == null) break;
                ex = ex.InnerException;
            }

            return text.ToString();
        }
    }
}
