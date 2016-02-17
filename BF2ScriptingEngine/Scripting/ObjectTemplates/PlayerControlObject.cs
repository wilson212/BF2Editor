using System;
using System.Collections.Generic;
using System.Text;
using BF2ScriptingEngine.Scripting.Attributes;
using BF2ScriptingEngine.Scripting.Components;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// This object is what Battlefield 2 uses as a base object for vehicles and 
    /// stationary weapons, or whatever the player can "enter". The PlayerControlObject 
    /// itself contain information like Hitpoints and damage information, 
    /// ammo/weapon icons, and other miscellaneous properties.
    /// </summary>
    /// <remarks>
    /// Most of the actual behavior for vehicles and stationary weapons is determined by the child objects. 
    /// In simple vehicles (like stationary weapons), most of the objects are direct children, but in complex 
    /// vehicles (tanks, jeeps, planes, ships) the only direct children of a PlayerControlObject are FloatingBundle t
    /// ype objects (for ships, amphibious vehicles, and submarines) and a LodObject that contains a Bundle with 
    /// all of the physics, weapons, and other PlayerControlObjects (for multiple positions).
    /// </remarks>
    /// <seealso cref="http://bfmods.com/mdt/scripting/ObjectTemplate/Types/PlayerControlObject.html"/>
    /// <example>The PlayerControlObject object type is created by ObjectTemplate.Create.</example>
    public class PlayerControlObject : ObjectTemplate
    {
        #region Properties

        /// <summary>
        /// Gets or Sets  the amount of air resistance the aircraft generates. 
        /// The higher the value, the slower the vehicle. This property can 
        /// also be used in effects.
        /// </summary>
        [PropertyName("drag")]
        public ObjectProperty<double> Drag { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("dragModifier")]
        public ObjectProperty<string> DragModifier { get; internal set; }

        /// <summary>
        /// Gets or Sets the weight of the object in kilograms. It will oppose 
        /// any lift due to gravity.
        /// </summary>
        [PropertyName("mass")]
        public ObjectProperty<int> Mass { get; internal set; }

        /// <summary>
        /// This property defines the location and rotation where a soldier 
        /// appears when he exits the vehicle, relative to the center of 
        /// the PlayerControlObject.
        /// </summary>
        /// <remarks>
        /// Argument 1: Location, releative to the object center, as a Point3D
        /// Argument 2: Soldier Rotation on exit, as a Point3D
        /// </remarks>
        [PropertyName("setSoldierExitLocation")]
        public ObjectProperty<Point3D, Point3D> SoldierExitLocation { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("seatInformation")]
        public ObjectProperty<string, Point3D, Point3D> SeatInformation { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("seatAnimationSystem")]
        public ObjectProperty<string> SeatAnimationSystem { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("soundFilter")]
        public ObjectProperty<string> SoundFilter { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("sprintFactor")]
        public ObjectProperty<int> SprintFactor { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("specialToggleWeaponInput")]
        public ObjectProperty<PlayerInput> SpecialToggleWeaponInput { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("vehicleCategory")]
        public ObjectProperty<VehicleCategory> VehicleCategory { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("controlsCategory")]
        public ObjectProperty<ControlsCategory> ControlsCategory { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("listenerObstruction")]
        public ObjectProperty<double> ListenerObstruction { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("damagedAmbientSoundLimit")]
        public ObjectProperty<double> DamagedAmbientSoundLimit { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("crewKitIndex")]
        public ObjectProperty<int> CrewKitIndex { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("autoUseAbility")]
        public ObjectProperty<bool> AutoUseAbility { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("hasRestrictedExit")]
        public ObjectProperty<bool> HasRestrictedExit { get; internal set; }

        #endregion

        #region Components

        /// <summary>
        /// Gets the Armor Componenet
        /// </summary>
        [Component("Armor")]
        [PropertyName("armor")]
        [Comment(
            Before = "---BeginComp:Armor ---"
        //After = "---EndComp ---"
        )]
        public ObjectProperty<Armor> Armor { get; set; }

        /// <summary>
        /// Gets the Radio Hud Component
        /// </summary>
        [Component("Radio")]
        [PropertyName("Radio")]
        [Comment(
            Before = "---BeginComp:Radio ---"
        //After = "---EndComp ---"
        )]
        public ObjectProperty<Radio> Radio { get; set; }

        /// <summary>
        /// Gets the Vehicle Hud Component
        /// </summary>
        [Component("VehicleHud")]
        [PropertyName("vehicleHud")]
        [Comment(
            Before = "---BeginComp:VehicleHud ---"
        //After = "---EndComp ---"
        )]
        public ObjectProperty<VehicleHud> VehicleHud { get; set; }

        /// <summary>
        /// Gets the Warning Hud Component
        /// </summary>
        [Component("WarningHud")]
        [PropertyName("WarningHud")]
        [Comment(
            Before = "---BeginComp:WarningHud ---"
        //After = "---EndComp ---"
        )]
        public ObjectProperty<WarningHud> WarningHud { get; set; }

        /// <summary>
        /// Gets the Warning Hud Component
        /// </summary>
        [Component("HelpHud")]
        [PropertyName("HelpHud")]
        [Comment(
            Before = "---BeginComp:HelpHud ---"
        //After = "---EndComp ---"
        )]
        public ObjectProperty<HelpHud> HelpHud { get; set; }

        #endregion

        public PlayerControlObject(string Name, Token Token) : base(Name, Token)
        { 

        }
    }
}
