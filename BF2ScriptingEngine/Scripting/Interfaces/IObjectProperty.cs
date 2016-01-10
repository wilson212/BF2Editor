using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    interface IObjectProperty
    {
        string GetName();

        Token GetToken();

        object GetValue();
    }
}
