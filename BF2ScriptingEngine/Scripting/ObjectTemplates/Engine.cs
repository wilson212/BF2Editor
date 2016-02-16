using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// The Engine object in Battlefield 2 is used to genereate 
    /// thrust for vehicles, weapons, and other objects.
    /// </summary>
    public class Engine : ObjectTemplate
    {
        public Engine(string name, Token token) : base(name, token)
        {

        }
    }
}
