using System;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting.Components
{
    public class HelpHud : ConFileObject, IComponent
    {
        /// <summary>
        /// 
        /// </summary>
        [PropertyName("helpStringKey")]
        public ObjectProperty<string> HelpStringKey { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("helpSoundKey")]
        public ObjectProperty<string> HelpSoundKey { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("lowArmorHelpStringKey")]
        public ObjectProperty<string> LowArmorHelpStringKey { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("lowArmorHelpSoundKey")]
        public ObjectProperty<string> LowArmorHelpSoundKey { get; internal set; }

        public HelpHud(string name, Token token) : base("HelpHud", token)
        {

        }
    }
}
