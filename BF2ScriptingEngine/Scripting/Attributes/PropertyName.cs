using System;

namespace BF2ScriptingEngine.Scripting.Attributes
{
    /// <summary>
    /// This attribute represents the proper name (camel case)
    /// for a property in a .con or .ai file
    /// </summary>
    /// <remarks>
    /// The script engine uses this attribute to map confile
    /// properties to a C# object properties
    /// </remarks>
    public class PropertyName : Attribute
    {
        /// <summary>
        /// The name of this property and all of its aliases, 
        /// using the proper camel case notation
        /// </summary>
        public string[] Names { get; protected set; }

        /// <summary>
        /// Creates a new instance of PropertyName
        /// </summary>
        /// <param name="name"></param>
        public PropertyName(params string[] names)
        {
            this.Names = names;
        }
    }
}
