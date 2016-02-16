using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting.Components
{
    public class DefaultAmmoComp : ConFileObject, IComponent
    {
        public DefaultAmmoComp(string name, Token token) : base("DefaultAmmoComp", token)
        {

        }
    }
}
