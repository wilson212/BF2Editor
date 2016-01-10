using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Returns the Type of elements stored in this Enumerable
        /// </summary>
        public static T GetEnumeratedType<T>(this IEnumerable<T> _)
        {
            return default(T);
        }
    }
}
