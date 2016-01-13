using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    public class AiTemplate : ConFileObject
    {
        /// <summary>
        /// Contains a list of Object types that dsecribe this object
        /// </summary>
        [PropertyName("addType")]
        public ObjectProperty<List<string>> Types;

        /// <summary>
        /// Gets or Sets how fast the bots forget the object once they cannot see it anymore. 
        /// </summary>
        /// <remarks>Format appears to be in seconds</remarks>
        [PropertyName("degeneration")]
        public ObjectProperty<int> Degeneration;

        /// <summary>
        /// This command is used to tell the bots how often they should update the information they have 
        /// about the object while it is in view.
        /// </summary>
        /// <remarks>
        /// This is an optimisation since it takes some time to update an object, at the 
        /// same time, the bot’s information should not grow too old, or they will start making
        /// erroneous decisions based on it.
        /// </remarks>
        [PropertyName("allowedTimeDiff")]
        public ObjectProperty<double> AllowedTimeDiff;

        /// <summary>
        /// Gets or Sets the basic temperature of this template. The higher the
        /// temperature, the more likely the bot will prioritize this object over
        /// other objects and actions
        /// </summary>
        [PropertyName("basicTemp")]
        public ObjectProperty<int> BasicTemp;

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("commonKnowledge")]
        public ObjectProperty<bool> CommonKnowledge;

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("secondary")]
        public ObjectProperty<bool> Secondary;

        /// <summary>
        /// Contains a list of AiTemplatePlugins for this object
        /// </summary>
        [PropertyName("addPlugIn")]
        public ObjectProperty<List<string>> Plugins;

        /// <summary>
        /// Creates a new instance of AiTemplate
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="ConFile"></param>
        /// <param name="Token"></param>
        public AiTemplate(string Name, Token Token) : base(Name, "aiTemplate", Token) { }

        /// <summary>
        /// Creates a new instance of AiTemplate with the following attributes
        /// </summary>
        /// <param name="tokenArgs">The command line token</param>
        /// <param name="Token">The ConFile token</param>
        public static AiTemplate Create(TokenArgs tokenArgs, Token Token)
        {
            return new AiTemplate(tokenArgs.Arguments.Last(), Token);
        }
    }
}
