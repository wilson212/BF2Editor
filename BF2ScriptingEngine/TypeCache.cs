using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine
{
    internal static class TypeCache
    {
        /// <summary>
        /// Our namespace cache
        /// </summary>
        internal static Dictionary<string, Type[]> Cache = new Dictionary<string, Type[]>();

        /// <summary>
        /// Returns all types found within the specified namespace
        /// </summary>
        /// <param name="nameSpace"></param>
        /// <returns></returns>
        internal static Type[] GetTypesInNamespace(string nameSpace)
        {
            // Check cache for stored types
            if (!Cache.ContainsKey(nameSpace))
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Cache[nameSpace] = assembly.GetTypes().Where(
                    t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)
                ).ToArray();
                
            }

            return Cache[nameSpace];
        }
    }
}
