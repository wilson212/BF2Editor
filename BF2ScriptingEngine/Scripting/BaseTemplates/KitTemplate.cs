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
        protected ObjectProperty<List<int>> Strengths { get; set; }

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
        /// Gets or Sets the Weights assigned to chance of bots selecting this type target
        /// </summary>
        [PropertyName("setBattleStrength")]
        public ObjectProperty<Dictionary<StrengthType, double>> BattleStrengths { get; set; }

        /// <summary>
        /// Creates a new isntance of KitTemplate
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Token"></param>
        public KitTemplate(string Name, Token Token) : base(Name, "kitTemplate", Token) { }

        /// <summary>
        /// Creates a new instance of WeaponTemplate with the following attributes
        /// </summary>
        /// <param name="tokenArgs">The command line token</param>
        /// <param name="Token">The ConFile token</param>
        public static KitTemplate Create(TokenArgs tokenArgs, Token Token)
        {
            return new KitTemplate(tokenArgs.Arguments.Last(), Token);
        }
    }
}
