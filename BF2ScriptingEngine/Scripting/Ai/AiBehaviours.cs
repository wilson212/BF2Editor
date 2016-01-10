using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    public static class AiBehaviours
    {
        /// <summary>
        /// Internal variable to determine whether we have a file loaded
        /// </summary>
        private static bool isLoaded = false;

        /// <summary>
        /// Indicates wheteher the AiBehaviours file is loaded and parsed
        /// </summary>
        public static bool IsLoaded
        {
            get { return isLoaded; }
        }

        /// <summary>
        /// The number of Behaviors found in the AiBehaviours.ai
        /// </summary>
        public static int MaxNumberOfBehaviours;

        /// <summary>
        /// A full list of supported Behaviour Types
        /// </summary>
        public static string[] BehaviourTypes;

        /// <summary>
        /// A list of UnitWeights found in the AiDefaults.ai file
        /// </summary>
        public static List<AiBehaviourWeights> RegisteredUnitWeights = new List<AiBehaviourWeights>();

        /// <summary>
        /// The Unit Weight of the Man Downed state
        /// </summary>
        public static AiBehaviourWeights ManDownEquipment;

        /// <summary>
        /// The basic weight for bots
        /// </summary>
        public static AiBehaviourWeights BasicBotWeights;

        /// <summary>
        /// Vehicle name/type => vehicleObj
        /// </summary>
        public static Dictionary<string, VehicleType> VehicleTypes;

        /// <summary>
        /// A list of registered Interpreter Entries
        /// </summary>
        public static List<string> InterpreterEntries;

        /// <summary>
        /// Loads and parses an AiBehaviours.ai file
        /// </summary>
        /// <param name="FilePath"></param>
        public static void Load(string FilePath)
        {

        }
    }
}
