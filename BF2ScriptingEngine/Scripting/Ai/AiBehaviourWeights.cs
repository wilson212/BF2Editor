using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// Represents a UnitWeights
    /// </summary>
    public class AiBehaviourWeights
    {
        /// <summary>
        /// The Name of this UnitWeight
        /// </summary>
        public string Name = String.Empty;

        /// <summary>
        /// The list of supported weights by this vehicle type
        /// </summary>
        private Dictionary<string, double> InternalDic;

        /// <summary>
        /// Fetches the value of a Behaviour type in this weight, or 0.0 if the
        /// Behaviour type is not supported by this weight
        /// </summary>
        /// <param name="key">The Bahviour type</param>
        /// <returns></returns>
        public double this[string key]
        {
            get
            {
                return (InternalDic.ContainsKey(key)) ? InternalDic[key] : 0.0;
            }
            set
            {
                // Make sure the behavior is registered
                if (!AiBehaviours.BehaviourTypes.Contains(key))
                    throw new InvalidBehaviourException(key, "Invalid Behavior Type");

                // Add the behavior's value
                InternalDic[key] = value;
            }
        }

        /// <summary>
        /// Creates a new instance of AiBehaviourModifiers
        /// </summary>
        /// <param name="Name">The Name issued to this Weight</param>
        public AiBehaviourWeights(string Name)
        {
            InternalDic = new Dictionary<string, double>(AiBehaviours.MaxNumberOfBehaviours);
            this.Name = Name;
        }
    }

    /// <summary>
    /// This is a nice variant of the KeyNotFoundException. The original version 
    /// is very mean, because it refuses to tell us which key was responsible 
    /// for raising the exception.
    /// </summary>
    public class InvalidBehaviourException : Exception
    {
        public string BehaviourName { get; private set; }

        public InvalidBehaviourException(string key, string message) : base(message)
        {
            this.BehaviourName = key;
        }

        public InvalidBehaviourException(string key, string message, Exception innerException) : base(message)
        {
            this.BehaviourName = key;
        }
    }
}
