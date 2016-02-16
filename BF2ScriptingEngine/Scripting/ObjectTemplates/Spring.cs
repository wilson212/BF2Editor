using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// Springs can be thought of as "wheels" for the Battlefield 2 game engine
    /// </summary>
    /// <remarks>
    /// They are what actually produces the thrust for jeeps, tanks, and APCs, 
    /// as well as allowing planes to land and take off. 
    /// 
    /// Any dynamic, mobile object that needs to interact with the landscape should 
    /// have springs. 
    /// 
    /// Springs generate thrust by being children of an Engine object, to make 
    /// them roll, while others are simple passive and roll on their own. This 
    /// can be changed with the Grip property. 
    ///  
    /// Springs also must have a Geometry property with a proper collision model 
    /// to work correctly.
    /// </remarks>
    public class Spring : ObjectTemplate
    {
        public Spring(string name, Token token) : base(name, token)
        {

        }
    }
}
