using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// Represents a very basic string value to place into a confile
    /// </summary>
    public class ConFileStringEntry : ConFileEntry
    {
        public ConFileStringEntry(Token token)
        {
            Token = token;
        }
    }
}
