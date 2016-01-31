using System;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting
{
    public class Physical : AiTemplatePlugin
    {
        [PropertyName("setStrType")]
        public ObjectProperty<string> StrengthType { get; set; }

        /// <summary>
        /// Creates a new instance of Armament
        /// </summary>
        /// <param name="Name">The name of this object</param>
        /// <param name="Token">The parser token</param>
        public Physical(string Name, Token Token) : base(Name, Token) { }
    }
}
