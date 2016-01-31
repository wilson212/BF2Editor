using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine
{
    public static class StringExtensions
    {
        public static bool StartsWithAny(this string text, params string[] items)
        {
            foreach (string item in items)
            {
                if (text.StartsWith(item))
                    return true;
            }

            return false;
        }
    }
}
