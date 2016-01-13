using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting.Components
{
    public class Ability : ObjectTemplate, IComponent
    {
        [PropertyName("hasRepairingAbility")]
        public ObjectProperty<bool> HasRepairingAbility;

        [PropertyName("hasHealingAbility")]
        public ObjectProperty<bool> HasHealingAbility;

        [PropertyName("hasAmmoAbility")]
        public ObjectProperty<bool> HasAmmoAbility;

        public Ability(string name, Token token) : base(name, token)
        {

        }
    }
}
