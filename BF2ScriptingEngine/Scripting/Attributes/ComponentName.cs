using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting.Attributes
{
    public class ComponentName : Attribute
    {
        /// <summary>
        /// The name of this property, using camel case
        /// </summary>
        public string Name;

        /// <summary>
        /// Creates a new instance of PropertyName
        /// </summary>
        /// <param name="name"></param>
        public ComponentName(string name)
        {
            this.Name = name;
        }
    }
}
