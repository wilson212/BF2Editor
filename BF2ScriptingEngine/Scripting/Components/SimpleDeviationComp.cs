using System;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting.Components
{
    public class SimpleDeviationComp : ConFileObject, IComponent
    {
        /// <summary>
        /// Gets or Sets the minimum deviation for the weapon. 
        /// Deviation can never become less than this.
        /// </summary>
        [PropertyName("minDev")]
        public ObjectProperty<double> MinDeviation { get; internal set; }

        public SimpleDeviationComp(string name, Token token) 
            : base("SimpleDeviationComp", token)
        {

        }
    }
}
