using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// A camera object is what Battlefield 2 uses to control the players 
    /// viewing position inside a PlayerControlObject.
    /// </summary>
    /// <remarks>
    /// This is where the different types of camera view points (like the chase and external views) 
    /// are enabled/disabled, as well as the mouselook controls. Most of the properties are 
    /// similar to RotationalBundle.
    /// </remarks>
    public class Camera : ObjectTemplate
    {
        public Camera(string name, Token token) : base(name, token)
        {

        }
    }
}
