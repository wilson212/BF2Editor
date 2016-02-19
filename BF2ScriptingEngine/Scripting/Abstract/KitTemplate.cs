using System;
using System.Collections.Generic;
using System.Linq;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// Tells the AI bots what the various types of soldiers are worth and are good to fight against.
    /// </summary>
    public class KitTemplate : ConFileObject
    {
        /// <summary>
        /// Contains the offensive and defensive strengths of this unit
        /// </summary>
        /// <remarks>Protected variable. Use <paramref name="OffensiveStrategicStrength"/> and 
        /// <paramref name="DefensiveStrategicStrength"/> instead
        /// </remarks>
        [PropertyName("setStrategicStrength"), IndexedList]
        protected virtual ObjectPropertyList<int, int> Strengths { get; set; }

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
        /// Gets or Sets the Weights assigned to chance of bots selecting this type target
        /// </summary>
        [PropertyName("setBattleStrength")]
        public ObjectPropertyDict<StrengthType, double> BattleStrengths { get; set; }

        /// <summary>
        /// Creates a new isntance of KitTemplate
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Token"></param>
        public KitTemplate(string Name, Token Token) : base(Name, Token) { }

        /// <summary>
        /// Creates a new instance of KitTemplate with the following attributes
        /// </summary>
        /// <param name="tokenArgs">The command line token</param>
        /// <param name="token">The ConFile token</param>
        public static KitTemplate Create(Token token)
        {
            return new KitTemplate(token.TokenArgs.Arguments.Last(), token);
        }
    }
}
