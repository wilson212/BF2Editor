﻿using System;
using System.Collections.Generic;
using System.Linq;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting
{
    public class AiTemplate : ConFileObject
    {
        /// <summary>
        /// Contains a list of Object types that dsecribe this object
        /// </summary>
        [PropertyName("addType")]
        public virtual ObjectPropertyList<string> Types { get; set; }

        /// <summary>
        /// Gets or Sets how fast the bots forget the object once they cannot see it anymore. 
        /// </summary>
        /// <remarks>Format appears to be in seconds</remarks>
        [PropertyName("degeneration")]
        public virtual ObjectProperty<int> Degeneration { get; set; }

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
        public virtual ObjectProperty<double> AllowedTimeDiff { get; set; }

        /// <summary>
        /// Gets or Sets the basic temperature of this template. The higher the
        /// temperature, the more likely the bot will prioritize this object over
        /// other objects and actions
        /// </summary>
        [PropertyName("basicTemp")]
        public virtual ObjectProperty<int> BasicTemp { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("commonKnowledge")]
        public virtual ObjectProperty<bool> CommonKnowledge { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("secondary")]
        public virtual ObjectProperty<bool> Secondary { get; set; }

        /// <summary>
        /// Contains a list of AiTemplatePlugins for this object
        /// </summary>
        [PropertyName("addPlugIn")]
        public virtual ObjectPropertyList<string> Plugins { get; set; }

        /// <summary>
        /// Creates a new instance of AiTemplate
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="ConFile"></param>
        /// <param name="Token"></param>
        public AiTemplate(string Name, Token Token) : base(Name, Token) { }

        /// <summary>
        /// Creates a new instance of AiTemplate with the following attributes
        /// </summary>
        /// <param name="tokenArgs">The command line token</param>
        /// <param name="token">The ConFile token</param>
        public static AiTemplate Create(Token token)
        {
            return new AiTemplate(token.TokenArgs.Arguments.Last(), token);
        }
    }
}
