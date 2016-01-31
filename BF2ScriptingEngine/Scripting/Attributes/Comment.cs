using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting.Attributes
{
    /// <summary>
    /// This attribute tells the <see cref="ScriptEngine"/> to apply
    /// the specified Rem comments before and after the property
    /// </summary>
    public class Comment : Attribute
    {
        public string Before { get; set; }

        public string After { get; set; }
    }
}
