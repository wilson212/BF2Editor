using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// This attribute is used to indetify ObjectProperties that
    /// use an index ID for their values. This is used with <see cref="Dictionary{TKey, TValue}"/>
    /// as well as <see cref="List{T}"/> typed collections.
    /// </summary>
    /// <example>
    /// aiTemplatePlugIn.setStrategicStrength {index} {value}
    /// </example>
    public class IndexedList : Attribute { }
}
