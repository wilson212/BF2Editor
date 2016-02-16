using System;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting.Components
{
    public class Ability : ConFileObject, IComponent
    {
        [PropertyName("hasRepairingAbility")]
        public ObjectProperty<bool> HasRepairingAbility { get; set; }

        [PropertyName("hasHealingAbility")]
        public ObjectProperty<bool> HasHealingAbility { get; set; }

        [PropertyName("hasAmmoAbility")]
        public ObjectProperty<bool> HasAmmoAbility { get; set; }

        public Ability(string name, Token token) : base(name, token)
        {

        }
    }
}
