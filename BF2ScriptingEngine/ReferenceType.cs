using System;
using System.Collections.Generic;
using BF2ScriptingEngine.Scripting;

namespace BF2ScriptingEngine
{
    public class ReferenceType
    {
        /// <summary>
        /// The reference string used to reference this object entry
        /// </summary>
        /// <remarks>
        /// Ex: AiSettings.setMaxNumBots 32 :: AiSettings is the Reference Name
        /// Ex: [ReferenceName].[PropertyName] [Values seperated by a space/tab]
        /// </remarks>
        public string Name { get; set; }

        /// <summary>
        /// Determines whether this <see cref="ReferenceType"/> is
        /// static, and does not contain an object creation method
        /// </summary>
        public bool IsStatic
        {
            get { return Mappings.Count == 0; }
        }

        /// <summary>
        /// Gets the <see cref="System.Type"/> of this <see cref="ReferenceType"/>
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Contains a list of object creation mappings
        /// </summary>
        public Dictionary<string, Func<Token, ConFileObject>> Mappings { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="ReferenceType"/>
        /// </summary>
        /// <param name="name">The name of this base type class</param>
        /// <param name="type">The <see cref="System.Type"/> of this base class type</param>
        public ReferenceType(string name, Type type)
        {
            Name = name;
            Type = type;
            Mappings = new Dictionary<string, Func<Token, ConFileObject>>();
        }

        /// <summary>
        /// Returns the object creation method by propertyName
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Func<Token, ConFileObject> GetMethod(string name)
        {
            return Mappings[name];
        }

        /// <summary>
        /// Returns whether an object is equal to this object
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is ReferenceType)
            {
                ReferenceType type = obj as ReferenceType;
                var Comparer = StringComparer.InvariantCulture;
                return Comparer.Equals(this.Name, type.Name);
            }

            return false;
        }

        /// <summary>
        /// Gets the hash code of this objects name
        /// </summary>
        public override int GetHashCode()
        {
            var Comparer = StringComparer.InvariantCulture;
            return Comparer.GetHashCode(Name);
        }

        public override string ToString() => Name;
    }
}
