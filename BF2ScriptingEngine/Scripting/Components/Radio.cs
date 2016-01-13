using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting.Components
{
    public class Radio : ObjectTemplate, IComponent
    {
        [PropertyName("spottedMessage")]
        public ObjectProperty<string> SpottedMessage;

        public Radio(string name, Token token) : base(name, token)
        {

        }
    }
}
