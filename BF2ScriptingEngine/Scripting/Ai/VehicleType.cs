using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// Represents a Vehicle type listed in the AiBehaviours.ai file
    /// </summary>
    public class VehicleType
    {
        /// <summary>
        /// The vehucles ClusterMapping
        /// </summary>
        public string ClusterMapping;

        /// <summary>
        /// The Unit Weights for this vehicle type
        /// </summary>
        public AiBehaviourWeights BehaviourModifier;

        /// <summary>
        /// The Material costs for this vehicle
        /// </summary>
        public MaterialCosts MaterialCost { get; private set; }

        /// <summary>
        /// A list of Vehicle Behaviour's and their parameters
        /// </summary>
        public Dictionary<string, object[]> BehaviourParams;

        /// <summary>
        /// The default behaviour for this vehicle type
        /// </summary>
        public string DefaultBehaviourName; // Mostly Idle

        /// <summary>
        /// A list of found Interpreter Entries for this vehicle
        /// </summary>
        public List<string> InterpreterEntries;
	    
        /// <summary>
        /// A list of found Default Interpreters for this vehicle
        /// </summary>
	    public List<string> DefaultInterpreters;
	    
        /// <summary>
        /// The listed sensing pattern
        /// </summary>
	    public string SensingAgentPattern = null; // Can be null

        public VehicleType()
        {
            MaterialCost = new MaterialCosts();
        }
    }

    public class MaterialCosts
    {
        public double Ground = 0.0;

        public double Road = 0.0;

        public double Shallows = 0.0;

        public double DeepWater = 0.0;
    }
}
