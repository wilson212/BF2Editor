using System;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting.Components
{
    public class HelpHud : ConFileObject, IComponent
    {
        [PropertyName("helpStringKey")]
        public ObjectProperty<string> HelpStringKey { get; set; }

        [PropertyName("helpSoundKey")]
        public ObjectProperty<string> HelpSoundKey { get; set; }

        public HelpHud(string name, Token token) : base(name, token)
        {

        }
    }
}
