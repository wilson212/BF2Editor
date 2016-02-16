using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// A RotationalBundle is an object that can pivot based on user input, 
    /// or at a continuous speed. It can also have child objects, such as 
    /// weapons and springs, making it ideal for turrets and steering.
    /// </summary>
    public class RotationalBundle : ObjectTemplate
    {
        public RotationalBundle(string name, Token token) : base(name, token)
        {

        }
    }
}
