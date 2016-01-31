using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting.Attributes
{
    /// <summary>
    /// This attribute tells the ScriptEngine to quote the
    /// values at the specified indexes, when converting the
    /// property to script format
    /// </summary>
    public class Quoted : Attribute
    {
        /// <summary>
        /// The list of indexes that will be quoted
        /// </summary>
        public List<int> Indexes { get; set; }

        public Quoted(params int[] indexes)
        {
            Indexes = new List<int>(indexes);
            if (Indexes.Count == 0)
                Indexes.Add(0);
        }
    }
}
