using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting.Components
{
    public class SimpleDeviationComp : ConFileObject, IComponent
    {
        public SimpleDeviationComp(string name, Token token) 
            : base("SimpleDeviationComp", token)
        {

        }
    }
}
