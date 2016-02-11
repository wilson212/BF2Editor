using System;
using System.Collections.Generic;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting.Components
{
    public class VehicleHud : ObjectTemplate, IComponent
    {
        /// <summary>
        /// Gets or Sets what the vehicle will be called in the Hud.
        /// </summary>
        [PropertyName("hudName"), Quoted]
        public ObjectProperty<string> HudName { get; set; }

        /// <summary>
        /// Hud graphics information.
        /// </summary>
        [PropertyName("miniMapIcon"), Quoted]
        public ObjectProperty<string> MiniMapIcon { get; set; }

        /// <summary>
        /// Hud graphics information.
        /// </summary>
        [PropertyName("miniMapIconLeaderSize")]
        public ObjectProperty<MapIconSize> MiniMapIconLeaderSize { get; set; }

        [PropertyName("usePlayerIcon"), Quoted]
        public ObjectProperty<bool> UsePlayerIcon { get; set; }

        /// <summary>
        /// Hud graphics information.
        /// </summary>
        [PropertyName("vehicleIcon"), Quoted]
        public ObjectProperty<string> VehicleIcon { get; set; }

        /// <summary>
        /// Hud graphics information.
        /// </summary>
        [PropertyName("vehicleIconPos")]
        public ObjectProperty<string> VehicleIconPos { get; set; }

        /// <summary>
        /// Hud graphics information.
        /// </summary>
        [PropertyName("abilityIcon"), Quoted]
        public ObjectProperty<string> AbilityIcon { get; set; }

        /// <summary>
        /// Hud graphics information.
        /// </summary>
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