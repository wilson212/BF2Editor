using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting.Components
{
    public class WeaponBasedRecoilComp : ConFileObject, IComponent
    {
        public WeaponBasedRecoilComp(string name, Token token) 
            : base("WeaponBasedRecoilComp", token)
        {

        }
    }
}
