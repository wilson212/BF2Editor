using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// Defines what geometry to use to render a type of object.
    /// </summary>
    public abstract class GeometryTemplate : ConFileObject
    {
        /// <summary>
        /// Gets or Sets the LOD (Level Of Detail) of the model will be 
        /// seen at a particular distance.
        /// </summary>
        /// <remarks>
        /// Based on my research, which may not 100% accurate (lack of documentation)
        /// 
        /// First param indetifies the Mesh part
        /// Second Param represents the LOD id of the mesh
        /// Thrid param is the actual Distance the mesh comes into view
        /// </remarks>
        /// <seealso cref="http://bfmods.com/mdt/scripting/GeometryTemplate/Properties/SetLodDistance.html"/>
        [PropertyName("setSubGeometryLodDistance")]
        public ObjectPropertyList<int, int, int> SubGeometryLodDistances { get; internal set; } 

        #region Mappings

        /// <summary>
        /// Contains a Mapping of object types, that derive from <see cref="GeometryTemplate"/>
        /// </summary>
        public static Dictionary<string, Type> ObjectTypes { get; set; }

        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="GeometryTemplate"/>
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Token"></param>
        public GeometryTemplate(string Name, Token Token) : base(Name, Token) { }

        static GeometryTemplate()
        {
            // Create object mappings
            var Comparer = StringComparer.InvariantCultureIgnoreCase;
            Type baseType = typeof(GeometryTemplate);
            Type[] typelist = TypeCache.GetTypesInNamespace("BF2ScriptingEngine.Scripting")
                .Where(x => baseType.IsAssignableFrom(x)).ToArray();
            ObjectTypes = typelist.ToDictionary(x => x.Name, v => v, Comparer);
        }

        /// <summary>
        /// Creates a new instance of GeometryTemplate with the following attributes
        /// </summary>
        /// <param name="tokenArgs">The command line token</param>
        /// <param name="token">The ConFile token</param>
        public static ConFileObject Create(Token token)
        {
            // Make sure we have the correct number of arguments
            if (token.TokenArgs.Arguments.Length != 2)
            {
                throw new ArgumentException(String.Concat(
                    "Invalid arguments count for GeometryTemplate;",
                     $"Got {token.TokenArgs.Arguments.Length}, Expecting 2."
                ));
            }

            // Extract our arguments
            string type = token.TokenArgs.Arguments[0];
            string name = token.TokenArgs.Arguments[1];

            // Ensure this type is supported
            if (!ObjectTypes.ContainsKey(type))
                throw new ParseException("Invalid GeometryTemplate derived type \"" + type + "\".", token);

            // Create and return our object instance
            var t = ObjectTypes[type];
            return (ConFileObject)Activator.CreateInstance(t, name, token);
        }
    }
}
