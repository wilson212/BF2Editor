using System;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting.Components
{
    public class Armor : ObjectTemplate, IComponent
    {
        #region Object Properties

        [PropertyName("maxHitPoints")]
        public ObjectProperty<int> MaxHitPoints { get; internal set; }

        [PropertyName("hitPoints")]
        public ObjectProperty<int> HitPoints { get; internal set; }

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
        public ObjectProperty<int> WaterDamageDelay { get; internal set; }

        [PropertyName("deepWaterDamageDelay")]
        public ObjectProperty<int> DeepWaterDamageDelay { get; internal set; }

        [PropertyName("waterLevel")]
        public ObjectProperty<double> WaterLevel { get; internal set; }

        [PropertyName("deepWaterLevel")]
        public ObjectProperty<double> DeepWaterLevel { get; internal set; }

        [PropertyName("explosionForce")]
        public ObjectProperty<int> ExplosionForce { get; internal set; }

        [PropertyName("explosionForceMod")]
        public ObjectProperty<int> ExplosionForceMod { get; internal set; }

        [PropertyName("explosionForceMax")]
        public ObjectProperty<int> ExplosionForceMax { get; internal set; }

        [PropertyName("explosionDamage")]
        public ObjectProperty<int> ExplosionDamage { get; internal set; }

        [PropertyName("explosionRadius")]
        public ObjectProperty<int> ExplosionRadius { get; internal set; }

        [PropertyName("explosionMaterial")]
        public ObjectProperty<int> ExplosionMaterial { get; internal set; }

        [PropertyName("wreckExplosionForce")]
        public ObjectProperty<int> WreckExplosionForce { get; internal set; }

        [PropertyName("wreckExplosionDamage")]
        public ObjectProperty<int> WreckExplosionDamage { get; internal set; }

        [PropertyName("wreckExplosionRadius")]
        public ObjectProperty<int> WreckExplosionRadius { get; internal set; }

        [PropertyName("wreckExplosionMaterial")]
        public ObjectProperty<int> WreckExplosionMaterial { get; internal set; }

        [PropertyName("wreckHitPoints")]
        public ObjectProperty<int> WreckHitPoints { get; internal set; }

        [PropertyName("timeToStayAsWreck")]
        public ObjectProperty<int> TimeToStayAsWreck { get; internal set; }

        [PropertyName("criticalDamage")]
        public ObjectProperty<int> CriticalDamage { get; internal set; }

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
        public ObjectMethod<int, string, string, string> AddArmorEffect { get; internal set; }

        #endregion

        public Armor(string name, Token token) : base(name, token)
        {
            // Instantiate Object Methods
            AddArmorEffect = new ObjectMethod<int, string, string, string>(Action_AddArmorEffect);
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
        private void Action_AddArmorEffect(Token token, int hitPoints, string name, string position, string rotation)
        {
            throw new NotImplementedException();
        }
    }
}
