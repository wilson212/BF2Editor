using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// The landing gear system is similar to a <see cref="RotationalBundle"/>.
    /// It allows a geometry template to be rotated at a certain angle and speed. 
    /// However, the LandingGear type allows the input to be based on engine speed 
    /// or even height from the ground.
    /// </summary>
    public class LandingGear : ObjectTemplate
    {
        public LandingGear(string name, Token token) : base(name, token)
        {

        }
    }
}
