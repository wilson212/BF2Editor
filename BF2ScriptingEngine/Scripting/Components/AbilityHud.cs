using System;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting.Components
{
    public class AbilityHud : ConFileObject, IComponent
    {
        [PropertyName("healingSound")]
        public ObjectProperty<string> HealingSound { get; set; }

        [PropertyName("repairingSound")]
        public ObjectProperty<string> RepairingSound { get; set; }

        [PropertyName("ammoSound")]
        public ObjectProperty<string> AmmoSound { get; set; }

        public AbilityHud(string name, Token token) : base(name, token)
        {

        }
    }
}
