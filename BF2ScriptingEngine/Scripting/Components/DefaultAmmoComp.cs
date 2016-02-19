using System;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting.Components
{
    public class DefaultAmmoComp : ConFileObject, IComponent
    {
        /// <summary>
        ///  
        /// </summary>
        [PropertyName("ammoType")]
        public ObjectProperty<int> AmmoType { get; internal set; }

        /// <summary>
        /// Gets or Sets the number of magazines in soldier 
        /// or vehicle inventory when fully loaded.
        /// </summary>
        [PropertyName("nrOfMags")]
        public ObjectProperty<int> NumberOfMags { get; internal set; }

        /// <summary>
        /// Gets or Sets the number of shots in 1 magazine
        /// </summary>
        [PropertyName("magSize")]
        public ObjectProperty<int> MagSize { get; internal set; }

        /// <summary>
        /// Gets or Sets how long after loading sequence is initiated before the 
        /// weapon can be fired again (in seconds). Should be synced with reload 
        /// animation.
        /// </summary>
        [PropertyName("reloadTime")]
        public ObjectProperty<int> ReloadTime { get; internal set; }

        /// <summary>
        /// Gets or Sets whether the weapon will auto reload
        /// </summary>
        [PropertyName("autoReload")]
        public ObjectProperty<bool> AutoReload { get; internal set; }

        /// <summary>
        /// Gets or Sets whether the weapon will auto reload
        /// </summary>
        [PropertyName("reloadWithoutPlayer")]
        public ObjectProperty<bool> ReloadWithoutPlayer { get; internal set; }

        public DefaultAmmoComp(string name, Token token) : base("DefaultAmmoComp", token)
        {

        }
    }
}
