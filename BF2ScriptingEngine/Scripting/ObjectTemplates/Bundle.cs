using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// The <see cref="Bundle"/> is used to create a complex bundle of objects. 
    /// It allows you to add templates together to form entire vehicles structure.
    /// </summary>
    /// <remarks>
    /// A common use of a Bundle is to create the hull and add all the parts of the tank.
    /// </remarks>
    public class Bundle : ObjectTemplate
    {
        public Bundle(string name, Token token) : base(name, token)
        {

        }
    }
}
