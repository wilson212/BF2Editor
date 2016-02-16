using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// This object is used with a PlayerControlObject to create 
    /// points where players can enter vehicles.
    /// </summary>
    /// <remarks>
    /// A PlayerControlObject can have many entry points, so you 
    /// can set exactly where the person can enter depending on position.
    /// </remarks>
    public class EntryPoint : ObjectTemplate
    {
        public EntryPoint(string name, Token token) : base(name, token)
        {

        }
    }
}
