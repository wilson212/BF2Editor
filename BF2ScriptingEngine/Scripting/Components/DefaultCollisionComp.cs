using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting.Components
{
    public class DefaultCollisionComp : ObjectTemplate, IComponent
    {
        public DefaultCollisionComp(string name, Token token) : base(name, token)
        {

        }
    }
}
