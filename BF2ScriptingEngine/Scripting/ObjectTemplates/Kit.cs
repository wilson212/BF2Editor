using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BF2ScriptingEngine.Scripting.Components;

namespace BF2ScriptingEngine.Scripting.Objects
{
    /// <summary>
    /// W.I.P :: Not Currently Working
    /// </summary>
    public class Kit : ObjectTemplate
    {
        /// <summary>
        /// Increasing this value will decrease the time a soldier can run.
        /// </summary>
        [PropertyName("sprintStaminaDissipationFactor")]
        public ObjectProperty<double> SprintStaminaDissipationFactor;

        [PropertyName("abilityHud")]
        public ObjectProperty<AbilityHud> AbilityHud;

        [PropertyName("vehicleHud")]
        public ObjectProperty<VehicleHud> VehicleHud;

        /// <summary>
        /// Creates a new instance of VehicleHud
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Token"></param>
        public Kit(string Name, Token Token) : base(Name, Token) { }
    }
}
