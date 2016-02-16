using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting.Components
{
    public abstract class FireComp : ConFileObject, IComponent
    {
        public FireComp(string name, Token token) : base(name, token)
        {

        }
    }
}
