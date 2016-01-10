using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    public class Armament : AiTemplatePlugin
    {
        /// <summary>
        /// Sets the basic temprature of this template
        /// </summary>
        [PropertyName("setIsAntiAircraft")]
        public ObjectProperty<bool> IsAntiAircraft;

        /// <summary>
        /// Creates a new instance of Armament
        /// </summary>
        /// <param name="Name">The name of this object</param>
        /// <param name="Token">The parser token</param>
        public Armament(string Name, Token Token) : base(Name, Token) { }
    }
}
