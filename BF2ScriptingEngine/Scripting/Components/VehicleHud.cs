using System;
using System.Collections.Generic;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting.Components
{
    public class VehicleHud : ObjectTemplate, IComponent
    {
        [PropertyName("hudName"), Quoted]
        public ObjectProperty<string> HudName { get; set; }

        [PropertyName("miniMapIcon"), Quoted]
        public ObjectProperty<string> MiniMapIcon { get; set; }

        [PropertyName("miniMapIconLeaderSize")]
        public ObjectProperty<MapIconSize> MiniMapIconLeaderSize { get; set; }

        [PropertyName("usePlayerIcon"), Quoted]
        public ObjectProperty<bool> UsePlayerIcon { get; set; }

        [PropertyName("vehicleIcon"), Quoted]
        public ObjectProperty<string> VehicleIcon { get; set; }

        [PropertyName("vehicleIconPos")]
        public ObjectProperty<string> VehicleIconPos { get; set; }

        [PropertyName("abilityIcon"), Quoted]
        public ObjectProperty<string> AbilityIcon { get; set; }

        [PropertyName("spottedIcon"), Quoted]
        public ObjectProperty<string> SpottedIcon { get; set; }

        [PropertyName("pantingSound")]
        public ObjectProperty<string> PantingSound { get; set; }

        [PropertyName("injurySound")]
        public ObjectProperty<string> InjurySound { get; set; }

        [PropertyName("vehicleType")]
        public ObjectProperty<int> VehicleType { get; set; }

        [PropertyName("useSelectionIcons")]
        public ObjectProperty<int> UseSelectionIcons { get; set; }

        [PropertyName("useVehicleCommRose")]
        public ObjectProperty<bool> UseVehicleCommRose { get; set; }

        [PropertyName("standardHelpEnabled")]
        public ObjectProperty<bool> StandardHelpEnabled { get; set; }

        public VehicleHud(string name, Token token) : base(name, token)
        {

        }
    }
}