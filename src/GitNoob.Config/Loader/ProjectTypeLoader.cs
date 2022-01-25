using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace GitNoob.Config.Loader
{
    public static class ProjectTypeLoader
    {
        private static Dictionary<string, IProjectType> _types = new Dictionary<string, IProjectType>();

        public static IProjectType Load(string name)
        {
            name = name.ToLowerInvariant();
            if (!_types.ContainsKey(name)) return null;

            return _types[name];
        }

        public static void LoadProjectTypesAssembly(string fullname)
        {
            try
            {
                Assembly asm = Assembly.Load(File.ReadAllBytes(fullname));

                Type ti = typeof(IProjectType);
                foreach (Type t in asm.GetTypes())
                {
                    if (!ti.Equals(t) && ti.IsAssignableFrom(t))
                    {
                        _types.Add(t.Name.ToLowerInvariant(), (IProjectType)Activator.CreateInstance(t));
                    }
                }
            }
            catch { }
        }
    }
}
