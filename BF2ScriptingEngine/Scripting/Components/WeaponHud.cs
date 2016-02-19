using System;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting.Components
{
    public class WeaponHud : ConFileObject, IComponent
    {
        /// <summary>
        /// 
        /// </summary>
        [PropertyName("weaponIcon")]
        public ObjectProperty<string> WeaponIcon { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("selectIcon")]
        public ObjectProperty<string> SelectIcon { get; internal set; }

        /// <summary>
        /// Gets or Sets the name that will be displayed in the HUD 
        /// when using the weapon.
        /// </summary>
        [PropertyName("hudName")]
        public ObjectProperty<string> HudName { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("guiIndex")]
        public ObjectProperty<int> GuiIndex { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("hasFireRate")]
        public ObjectProperty<bool> HasFireRate { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("addShowOnCamMode")]
        public ObjectProperty<int> AddShowOnCamMode { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("showClips")]
        public ObjectProperty<bool> ShowClips { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("displaySelectOnActivation")]
        public ObjectProperty<bool> DisplaySelectOnActivation { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("overheatSound")]
        public ObjectProperty<string> OverheatSound { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("crosshairIcon")]
        public ObjectProperty<string> CrosshairIcon { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("crosshairIconSize")]
        public ObjectProperty<int> CrosshairIconSize { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("altCrosshairIcon")]
        public ObjectProperty<string> AltCrosshairIcon { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("enableMouse")]
        public ObjectProperty<bool> EnableMouse { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("enablePostProcessingOnGuiIndex")]
        public ObjectProperty<int> EnablePostProcessingOnGuiIndex { get; internal set; }

        public WeaponHud(string name, Token token) : base("WeaponHud", token)
        {

        }
    }
}
