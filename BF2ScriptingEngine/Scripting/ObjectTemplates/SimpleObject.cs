using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BF2ScriptingEngine.Scripting.GeometryTemplates;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// The SimpleObject in Battlefield 2 is a basic object that has 
    /// only one part, and no children. This object is mainly used for 
    /// static objects (chairs, tables, vegetation) and for details on 
    /// more complex objects (parts of vehicles, or effects).
    /// </summary>
    public class SimpleObject : ObjectTemplate
    {
        /// <summary>
        /// The SimpleObject cannot have any child templates attached!
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown Always for this class type!</exception>
        public override ObjectProperty<List<ChildTemplate>> Templates
        {
            get
            {
                return null; // Return null for script engine to ignore!
            } 
            internal set
            {
                throw new NotSupportedException("SimpleObject cannot contain any child objects.");
            }
        }

        /// <summary>
        /// Creates a new instance of Simple Object
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Token"></param>
        public SimpleObject(string Name, Token Token) : base(Name, Token) { }
    }
}
