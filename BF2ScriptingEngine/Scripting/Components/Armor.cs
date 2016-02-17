using System;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting.Components
{
    public class Armor : ConFileObject, IComponent
    {
        #region Object Properties

        /// <summary>
        /// Gets or Sets the maximum number of hitpoints the object can have.
        /// </summary>
        [PropertyName("maxHitPoints")]
        public ObjectProperty<int> MaxHitPoints { get; internal set; }

        /// <summary>
        /// Gets or Sets the Number of hitpoints the object has on spawn. 
        /// May not exceed MaxHitPoints.
        /// </summary>
        [PropertyName("hitPoints")]
        public ObjectProperty<int> HitPoints { get; internal set; }

        /// <summary>
        /// Gets or Sets the Material type object uses to recieve explosion damage. 
        /// 72 for "hard" objects, 71 for "soft".
        /// </summary>
        [PropertyName("defaultMaterial")]
        public ObjectProperty<int> DefaultMaterial { get; internal set; }

        [PropertyName("hpLostWhileUpSideDown")]
        public ObjectProperty<int> HpLostWhileUpSideDown { get; internal set; }

        [PropertyName("hpLostWhileInWater")]
        public ObjectProperty<int> HpLostWhileInWater { get; internal set; }

        [PropertyName("hpLostWhileInDeepWater")]
        public ObjectProperty<int> HpLostWhileInDeepWater { get; internal set; }

        [PropertyName("hpLostWhileCriticalDamage")]
        public ObjectProperty<int> HpLostWhileCriticalDamage { get; internal set; }

        [PropertyName("waterDamageDelay")]
        public ObjectProperty<double> WaterDamageDelay { get; internal set; }

        [PropertyName("deepWaterDamageDelay")]
        public ObjectProperty<double> DeepWaterDamageDelay { get; internal set; }

        [PropertyName("waterLevel")]
        public ObjectProperty<double> WaterLevel { get; internal set; }

        [PropertyName("deepWaterLevel")]
        public ObjectProperty<double> DeepWaterLevel { get; internal set; }

        /// <summary>
        /// Gets or Sets the force on movable objects the explosion applies.
        /// </summary>
        [PropertyName("explosionForce")]
        public ObjectProperty<int> ExplosionForce { get; internal set; }

        /// <summary>
        /// Normally 1
        /// </summary>
        [PropertyName("explosionForceMod")]
        public ObjectProperty<int> ExplosionForceMod { get; internal set; }

        [PropertyName("explosionForceMax")]
        public ObjectProperty<int> ExplosionForceMax { get; internal set; }

        /// <summary>
        /// Gets or Sets the max damage inflicted from explosion. Lessens over distance 
        /// from center of explosion to radius maximum.
        /// </summary>
        [PropertyName("explosionDamage")]
        public ObjectProperty<int> ExplosionDamage { get; internal set; }

        /// <summary>
        /// Gets or Sets the radius in meters object will radiate damage when it explodes.
        /// </summary>
        [PropertyName("explosionRadius")]
        public ObjectProperty<int> ExplosionRadius { get; internal set; }

        /// <summary>
        /// Gets or Sets what material the explosion radiates. Normally 70.
        /// </summary>
        [PropertyName("explosionMaterial")]
        public ObjectProperty<int> ExplosionMaterial { get; internal set; }

        /// <summary>
        /// Gets or Sets the force on movable objects the explosion applies.
        /// </summary>
        [PropertyName("wreckExplosionForce")]
        public ObjectProperty<int> WreckExplosionForce { get; internal set; }

        /// <summary>
        /// Gets or Sets the max damage inflicted from explosion. Lessens over 
        /// distance from center of explosion to radius maximum.
        /// </summary>
        [PropertyName("wreckExplosionDamage")]
        public ObjectProperty<int> WreckExplosionDamage { get; internal set; }

        /// <summary>
        /// Gets or Sets the radius in meters object will radiate damage when it explodes.
        /// </summary>
        [PropertyName("wreckExplosionRadius")]
        public ObjectProperty<int> WreckExplosionRadius { get; internal set; }

        /// <summary>
        /// Gets or Sets what material the explosion radiates. Normally 70.
        /// </summary>
        [PropertyName("wreckExplosionMaterial")]
        public ObjectProperty<int> WreckExplosionMaterial { get; internal set; }

        /// <summary>
        /// Gets or Sets the number of hitpoints the wreck has. May not exceed MaxHitPoints.
        /// </summary>
        [PropertyName("wreckHitPoints")]
        public ObjectProperty<int> WreckHitPoints { get; internal set; }

        /// <summary>
        /// Gets or Sets the Time it takes to hit 0 wreck hp once object has become a wreck 
        /// in seconds. Then the wreck too explodes.
        /// </summary>
        [PropertyName("timeToStayAsWreck")]
        public ObjectProperty<int> TimeToStayAsWreck { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("criticalDamage")]
        public ObjectProperty<int> CriticalDamage { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("destroyOnSpectacularDeath")]
        public ObjectProperty<bool> DestroyOnSpectacularDeath { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("hideChildrenOnSpectacularDeath")]
        public ObjectProperty<bool> HideChildrenOnSpectacularDeath { get; internal set; }

        #endregion

        #region Object Methods

        /// <summary>
        /// The AddArmorEffect property allows you to choose an effect when the 
        /// objects drops below a certain number of hitpoints. The effect will 
        /// stay on until it reaches the next armor effect.
        /// </summary>
        [PropertyName("addArmorEffect")]
        protected ObjectMethod<int, string, string, string> AddArmorEffect { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("addArmorEffectSpectacular")]
        protected ObjectMethod<int, string, string, string> AddArmorEffectSpectacular { get; set; }

        #endregion

        public Armor(string name, Token token) : base(name, token)
        {
            // Instantiate Object Methods
            AddArmorEffect = new ObjectMethod<int, string, string, string>(Method_AddArmorEffect);
            AddArmorEffectSpectacular = new ObjectMethod<int, string, string, string>(
                Method_AddArmorEffectSpectacular
            );
        }

        /// <summary>
        /// The AddArmorEffect property allows you to choose an effect when the 
        /// objects drops below a certain number of hitpoints. The effect will 
        /// stay on until it reaches the next armor effect.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="hitPoints"></param>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        private ConFileEntry Method_AddArmorEffect(
            Token token,
            int hitPoints,
            string name,
            string position,
            string rotation)
        {
            return new ConFileStringEntry(token);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="hitPoints"></param>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        private ConFileEntry Method_AddArmorEffectSpectacular(
            Token token, 
            int hitPoints, 
            string name, 
            string position, 
            string rotation)
        {
            return new ConFileStringEntry(token);
        }
    }
}
