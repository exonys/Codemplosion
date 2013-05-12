using System;
using System.Linq;
using System.Reflection;

namespace HelperLib
{
    public class Reflection
    {
        public static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            return
                assembly.GetTypes()
                        .Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal))
                        .ToArray();
        }
    }
}
