using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting.Components
{
    public class HelpHud : ObjectTemplate, IComponent
    {
        [PropertyName("helpStringKey")]
        public ObjectProperty<string> HelpStringKey;

        [PropertyName("helpSoundKey")]
        public ObjectProperty<string> HelpSoundKey;

        public HelpHud(string name, Token token) : base(name, token)
        {

        }
    }
}
