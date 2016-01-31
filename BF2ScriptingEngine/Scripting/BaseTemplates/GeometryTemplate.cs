using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BF2ScriptingEngine.Scripting.GeometryTemplates;

namespace BF2ScriptingEngine.Scripting
{
    public abstract class GeometryTemplate : ConFileObject
    {
        /// <summary>
        /// Creates a new isntance of WeaponTemplate
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Token"></param>
        public GeometryTemplate(string Name, Token Token) : base(Name, "GeometryTemplate", Token) { }

        /// <summary>
        /// Creates a new instance of WeaponTemplate with the following attributes
        /// </summary>
        /// <param name="tokenArgs">The command line token</param>
        /// <param name="Token">The ConFile token</param>
        public static GeometryTemplate Create(TokenArgs tokenArgs, Token Token)
        {
            string type = tokenArgs.Arguments[0];
            string name = tokenArgs.Arguments[1];

            switch (type.ToLowerInvariant())
            {
                case "bundledmesh": return new BundledMesh(name, Token);
                case "skinnedmesh": return new SkinnedMesh(name, Token);
                default:
                    throw new NotSupportedException("Invalid Object Type \"" + type + "\".");
            }
        }
    }
}
