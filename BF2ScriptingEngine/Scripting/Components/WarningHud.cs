using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting.Components
{
    public class WarningHud : ConFileObject, IComponent
    {
        /// <summary>
        /// Gets or Sets warning system to use.
        /// </summary>
        [PropertyName("warningType")]
        public ObjectProperty<int> WarningType { get; internal set; }

        /// <summary>
        /// Gets the name of a sound file.
        /// </summary>
        [PropertyName("firstWarningSound")]
        public ObjectProperty<string> FirstWarningSound { get; internal set; }

        /// <summary>
        /// Gets the name of a sound file.
        /// </summary>
        [PropertyName("secondWarningSound")]
        public ObjectProperty<string> SecondWarningSound { get; internal set; }

        /// <summary>
        /// Gets the name of aa Icon.
        /// </summary>
        [PropertyName("warningIcon")]
        public ObjectProperty<string> WarningIcon { get; internal set; }

        public WarningHud(string name, Token token) : base("WarningHud", token)
        {

        }
    }
}
