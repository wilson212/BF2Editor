using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting.Components
{
    public class VehicleHud : ConFileObject, IComponent
    {
        /// <summary>
        /// Creates a new instance of VehicleHud
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Token"></param>
        public VehicleHud(string Name, Token Token) : base(Name, "ObjectTemplate", Token) { }
    }
}
