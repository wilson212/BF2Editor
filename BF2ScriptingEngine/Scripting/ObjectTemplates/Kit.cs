using BF2ScriptingEngine.Scripting.Attributes;
using BF2ScriptingEngine.Scripting.Components;
using BF2ScriptingEngine.Scripting.GeometryTemplates;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// W.I.P :: Not Currently Working
    /// </summary>
    public class Kit : ObjectTemplate
    {
        #region Properties

        [PropertyName("kitType", 2)]
        public ObjectProperty<string> KitType { get; set; }

        #endregion

        #region Components

        /// <summary>
        /// The Ability Hud Component
        /// </summary>
        [Component("AbilityHud")]
        [PropertyName("abilityHud", 500)]
        [Comment(
            Before = "---BeginComp:AbilityHud ---",
            After = "---EndComp ---"
        )]
        public ObjectProperty<AbilityHud> AbilityHud { get; set; }

        /// <summary>
        /// The Vehicle Hud Component
        /// </summary>
        [Component("VehicleHud")]
        [PropertyName("vehicleHud", 502)]
        [Comment(
            Before = "---BeginComp:VehicleHud ---",
            After = "---EndComp ---"
        )]
        public ObjectProperty<VehicleHud> VehicleHud { get; set; }

        /// <summary>
        /// The Radio Hud Component
        /// </summary>
        [Component("Radio")]
        [PropertyName("Radio", 510)]
        [Comment(
            Before = "---BeginComp:Radio ---",
            After = "---EndComp ---"
        )]
        public ObjectProperty<Radio> Radio { get; set; }

        #endregion

        /// <summary>
        /// Increasing this value will decrease the time a soldier can run.
        /// </summary>
        [PropertyName("sprintStaminaDissipationFactor", 502)]
        public ObjectProperty<double> SprintStaminaDissipationFactor { get; set; }

        /// <summary>
        /// Creates a new instance of VehicleHud
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Token"></param>
        public Kit(string Name, Token Token) : base(Name, Token) { }
    }
}
