using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// An animated bundle is an object that has an animation associated 
    /// with it, e.g., a waving flag.
    /// </summary>
    /// <remarks>
    /// The geometry of the object does not necessarily have to be animated; 
    /// for example, a tank track is an animated bundle, though the only thing 
    /// that changes are the (u,v) coordinates of the track texture.
    /// </remarks>
    public class AnimatedBundle : ObjectTemplate
    {
        public AnimatedBundle(string name, Token token) : base(name, token)
        {

        }
    }
}
