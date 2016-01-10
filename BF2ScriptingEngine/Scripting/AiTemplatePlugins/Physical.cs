using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    public class Physical : AiTemplatePlugin
    {
        [PropertyName("setStrType")]
        public ObjectProperty<string> StrengthType;

        /// <summary>
        /// Creates a new instance of Armament
        /// </summary>
        /// <param name="Name">The name of this object</param>
        /// <param name="Token">The parser token</param>
        public Physical(string Name, Token Token) : base(Name, Token) { }
    }
}
