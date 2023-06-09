using System.Collections.Generic;
using System.Drawing;

namespace GitNoob.Utils
{
    public static class Resources
    {
        private static Dictionary<string, Icon> _icons = new Dictionary<string, Icon>();

        public static void setIcon(string name, Icon icon)
        {
            _icons.Add(name, icon);
        }

        public static Icon getIcon(string name)
        {
            Icon result;
            if (!_icons.TryGetValue(name, out result)) return null;

            return result;
        }

    }
}
