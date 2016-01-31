using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Extensions
{
    static class TypeExt
    {
        public static bool IsList(this Type type)
        {
            if (type.IsGenericType)
                return type.GetInterface("IList") != null;

            return false;
        }
    }
}
