using System;

namespace BF2ScriptingEngine.Scripting.Attributes
{
    /// <summary>
    /// This attribute represents the proper name (camel case) name
    /// for a property in a .con or .ai file
    /// </summary>
    public class PropertyName : Attribute
    {
        /// <summary>
        /// The name of this property, using camel case
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets the order weight of this property. Lower numbers will
        /// be ordered before higher ones.
        /// </summary>
        /// <remarks>
        /// By default, properties are ordered by when they are defined in the 
        /// C# object, but this does not account for derived properties like 
        /// from the <see cref="ObjectTemplate"/> class.
        /// </remarks>
        public int Priority { get; protected set; }

        /// <summary>
        /// Creates a new instance of PropertyName
        /// </summary>
        /// <param name="name"></param>
        public PropertyName(string name, int priority = 999)
        {
            this.Name = name;
            this.Priority = priority;
        }
    }
}
