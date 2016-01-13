using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting.Components
{
    public class VehicleHud : ObjectTemplate, IComponent
    {
        [PropertyName("hudName")]
        public ObjectProperty<string> HudName;

        [PropertyName("miniMapIcon")]
        public ObjectProperty<string> MiniMapIcon;

        [PropertyName("vehicleIcon")]
        public ObjectProperty<string> VehicleIcon;

        [PropertyName("abilityIcon ")]
        public ObjectProperty<string> AbilityIcon;

        [PropertyName("spottedIcon")]
        public ObjectProperty<string> SpottedIcon;

        [PropertyName("pantingSound")]
        public ObjectProperty<string> PantingSound;

        [PropertyName("injurySound")]
        public ObjectProperty<string> InjurySound;

        [PropertyName("vehicleType")]
        public ObjectProperty<int> VehicleType;

        public VehicleHud(string name, Token token) : base(name, token)
        {

        }
    }
}