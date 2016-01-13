using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        //public static GeometryTemplate Create(ObjectTokenArgs tokenArgs, Token Token)
        //{
            //return new GeometryTemplate(tokenArgs.Arguments.Last(), Token);
        //}
    }
}
