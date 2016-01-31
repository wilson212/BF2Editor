using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting.Attributes
{
    /// <summary>
    /// This attribute represents a Bf2 Component object
    /// </summary>
    public class Component : Attribute
    {
        /// <summary>
        /// The name of this component
        /// </summary>
        public string[] Types { get; protected set; }

        public Component(params string[] types)
        {
            Types = types;
        }
    }
}
