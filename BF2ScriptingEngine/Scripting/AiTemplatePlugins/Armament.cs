using System;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting
{
    public class Armament : AiTemplatePlugin
    {
        /// <summary>
        /// Sets the basic temprature of this template
        /// </summary>
        [PropertyName("setIsAntiAircraft")]
        public ObjectProperty<bool> IsAntiAircraft { get; set; }

        /// <summary>
        /// Creates a new instance of Armament
        /// </summary>
        /// <param name="Name">The name of this object</param>
        /// <param name="Token">The parser token</param>
        public Armament(string Name, Token Token) : base(Name, Token) { }
    }
}
