using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting.Components
{
    public abstract class ZoomComp : ConFileObject, IComponent
    {
        public ZoomComp(string name, Token token) : base(name, token)
        {

        }
    }
}
