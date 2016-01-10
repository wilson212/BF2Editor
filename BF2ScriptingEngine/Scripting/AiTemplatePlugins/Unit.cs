using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    public class Unit : AiTemplatePlugin
    {
        [PropertyName("equipmentTypeName")]
        public ObjectProperty<string> EquipmentTypeName;

        /// <summary>
        /// Contains the offensive and defensive strengths of this unit
        /// </summary>
        /// <remarks>Protected variable. Use <paramref name="OffensiveStrategicStrength"/> and 
        /// <paramref name="DefensiveStrategicStrength"/> instead
        /// </remarks>
        [PropertyName("setStrategicStrength"), IndexedList]
        protected ObjectProperty<List<int>> Strengths;

        [PropertyName("setSelectKey")]
        public ObjectProperty<string> SetSelectKey;

        [PropertyName("SelectKey")]
        public ObjectProperty<string> SelectKey;

        [PropertyName("cullDistance")]
        public ObjectProperty<int> CullDistance;

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>MAY BE NULL! Not all units define this variable!</remarks>
        [PropertyName("setHasExposedSoldier")]
        public ObjectProperty<bool> HasExposedSoldier;
        
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Found on the BiPod objects</remarks>
        [PropertyName("setUseNoPathfindingToGetToObject")]
        public ObjectProperty<bool> UseNoPathfindingToGetToObject;

        /// <summary>
        /// Gets or Sets the offensive strength of this Unit
        /// </summary>
        public int OffensiveStrategicStrength
        {
            get
            {
                // Ensure we have the proper amount of elements
                if (Strengths == null || Strengths.Value.Count < 2)
                    throw new Exception("Property not set");

                return Strengths.Value[0];
            }
            set
            {
                // Ensure we have the proper amount of elements
                if (Strengths == null || Strengths.Value.Count < 2)
                    throw new Exception("Property not set");

                Strengths.Value[0] = value;
            }
        }

        /// <summary>
        /// Gets or Sets the defensive strength of this Unit
        /// </summary>
        public int DefensiveStrategicStrength
        {
            get 
            {
                // Ensure we have the proper amount of elements
                if (Strengths == null || Strengths.Value.Count < 2)
                    throw new Exception("Property not set");

                return Strengths.Value[1]; 
            }
            set 
            {
                // Ensure we have the proper amount of elements
                if (Strengths == null || Strengths.Value.Count < 2)
                    throw new Exception("Property not set");

                Strengths.Value[1] = value; 
            }
        }

        /// <summary>
        /// Creates a new instance of Armament
        /// </summary>
        /// <param name="Name">The name of this object</param>
        /// <param name="Token">The parser token</param>
        public Unit(string Name, Token Token) : base(Name, Token) { }
    }
}
