using System;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting.Components
{
    public class Radio : ConFileObject, IComponent
    {
        [PropertyName("spottedMessage")]
        public ObjectProperty<string> SpottedMessage { get; set; }

        public Radio(string name, Token token) : base(name, token)
        {

        }
    }
}
