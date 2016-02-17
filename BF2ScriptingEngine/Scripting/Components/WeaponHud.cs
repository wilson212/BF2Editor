using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting.Components
{
    public class WeaponHud : ConFileObject, IComponent
    {
        public WeaponHud(string name, Token token) : base("WeaponHud", token)
        {

        }
    }
}
