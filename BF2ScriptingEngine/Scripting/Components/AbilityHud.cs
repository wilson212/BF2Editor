using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting.Components
{
    public class AbilityHud : ObjectTemplate, IComponent
    {
        [PropertyName("healingSound")]
        public ObjectProperty<string> HealingSound;

        [PropertyName("repairingSound")]
        public ObjectProperty<string> RepairingSound;

        [PropertyName("ammoSound")]
        public ObjectProperty<string> AmmoSound;

        public AbilityHud(string name, Token token) : base(name, token)
        {

        }
    }
}
