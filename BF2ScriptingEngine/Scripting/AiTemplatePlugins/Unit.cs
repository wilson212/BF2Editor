using System;
using System.Collections.Generic;
using System.Linq;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting
{
    public class Unit : AiTemplatePlugin
    {
        [PropertyName("equipmentTypeName")]
        public ObjectProperty<string> EquipmentTypeName { get; set; }

        /// <summary>
        /// Contains the offensive and defensive strengths of this unit
        /// </summary>
        /// <remarks>Protected variable. Use <paramref name="OffensiveStrategicStrength"/> and 
        /// <paramref name="DefensiveStrategicStrength"/> instead
        /// </remarks>
        [PropertyName("setStrategicStrength")]
        protected ObjectPropertyList<int, int> Strengths { get; set; }

        [PropertyName("setSelectKey")]
        public ObjectProperty<string> SetSelectKey { get; set; }

        [PropertyName("SelectKey")]
        public ObjectProperty<string> SelectKey { get; set; }

        [PropertyName("cullDistance")]
        public ObjectProperty<int> CullDistance { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>MAY BE NULL! Not all units define this variable!</remarks>
        [PropertyName("setHasExposedSoldier")]
        public ObjectProperty<bool> HasExposedSoldier { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Found on the BiPod objects</remarks>
        [PropertyName("setUseNoPathfindingToGetToObject")]
        public ObjectProperty<bool> UseNoPathfindingToGetToObject { get; set; }

        /// <summary>
        /// Gets or Sets the offensive strength of this Unit
        /// </summary>
        public int OffensiveStrategicStrength
        {
            get
            {
                // Ensure we have the proper amount of elements
                if (Strengths == null || Strengths.Items.Count < 2)
                    throw new Exception("Property not set");

                // Fetch the item
                var item = Strengths.Where(x => x.Value1 == 0).FirstOrDefault();
                return item.Value2;
            }
            set
            {
                // Ensure we have the proper amount of elements
                if (Strengths == null || Strengths.Items.Count < 2)
                    throw new Exception("Property not set");

                // Fetch the item
                var item = Strengths.Where(x => x.Value1 == 0).FirstOrDefault();
                item.Value2 = value;
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
                if (Strengths == null || Strengths.Items.Count < 2)
                    throw new Exception("Property not set");

                // Fetch the item
                var item = Strengths.Where(x => x.Value1 == 1).FirstOrDefault();
                return item.Value2;
            }
            set
            {
                // Ensure we have the proper amount of elements
                if (Strengths == null || Strengths.Items.Count < 2)
                    throw new Exception("Property not set");

                // Fetch the item
                var item = Strengths.Where(x => x.Value1 == 1).FirstOrDefault();
                item.Value2 = value;
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
