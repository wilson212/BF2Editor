using System;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting.Components
{
    public abstract class FireComp : ConFileObject, IComponent
    {
        /// <summary>
        /// Gets or Sets How fast the weapon can be fired. Value must be a factor of 1800."
        /// </summary>
        /// <remarks>
        /// 300, 360, 450, 600, 900, 1200, 1800
        /// </remarks>
        [PropertyName("roundsPerMinute")]
        public virtual ObjectProperty<int> RoundsPerMinute { get; internal set; }

        /// <summary>
        /// Gets or Sets the time in seconds from when you pull a trigger 
        /// to when the projectile is actually fired.
        /// </summary>
        [PropertyName("fireStartDelay")]
        public virtual ObjectProperty<CRD> FireStartDelay { get; internal set; }

        /// <summary>
        /// Use for hand fire arms. Fire the projectile from the center of 
        /// the crosshair as opposed to the center of the weapon model.
        /// </summary>
        [PropertyName("fireInCameraDof")]
        public virtual ObjectProperty<bool> FireInCameraDof { get; internal set; }

        public FireComp(string name, Token token) : base(name, token)
        {

        }
    }
}
