using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BF2ScriptingEngine.Scripting.Attributes;
using BF2ScriptingEngine.Scripting.GeometryTemplates;

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

        /// <summary>
        /// Creates a new isntance of WeaponTemplate
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Token"></param>
        public GeometryTemplate(string Name, Token Token) : base(Name, Token) { }

        /// <summary>
        /// Creates a new instance of GeometryTemplate with the following attributes
        /// </summary>
        /// <param name="tokenArgs">The command line token</param>
        /// <param name="token">The ConFile token</param>
        public static GeometryTemplate Create(Token token)
        {
            string type = token.TokenArgs.Arguments[0];
            string name = token.TokenArgs.Arguments[1];

            switch (type.ToLowerInvariant())
            {
                case "bundledmesh": return new BundledMesh(name, token);
                case "skinnedmesh": return new SkinnedMesh(name, token);
                default:
                    throw new NotSupportedException("Invalid Object Type \"" + type + "\".");
            }
        }
    }
}
