using System;
using System.Collections.Generic;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting
{
    public class ChildTemplate : ConFileObject
    {
        /// <summary>
        /// Gets or Sets the position of a child object attached to the current 
        /// object with the AddTemplate command.
        /// </summary>
        /// <remarks>
        /// This position is relative to the center of the parent object. 
        /// If this property is omitted, the position is 0/0/0.
        /// </remarks>
        /// <seealso cref="http://bfmods.com/mdt/scripting/ObjectTemplate/Properties/SetPosition.html"/>
        [PropertyName("setPosition")]
        public ObjectProperty<string> SetPosition;

        /// <summary>
        /// Gets or Sets the direction and angles of a child object attached to the 
        /// current object by the AddTemplate command. The angle is in terms of 
        /// yaw/pitch/roll. 
        /// </summary>
        /// <remarks>
        /// The units used for this parameter are degrees (1 - 360) but you can have 
        /// negative degrees as well. If you define multiple rotations, they happen 
        /// in this order.
        /// </remarks>
        /// <example>
        /// You are looking down the Z axis and Y is up, X to the right. Imagine turning 
        /// your head to left, then the right. Your head is rotating in the XZ axis, the 
        /// first parameter (aka yaw). Now imagine looking up, then down. Now your head is 
        /// rotating in the YZ axis, the second parameter (aka pitch). Now, keep looking forward, 
        /// but tilt your head to the left, then right. It is rotating in the XY axis, the 
        /// last parameter (aka roll).
        /// </example>
        /// <seealso cref="http://bfmods.com/mdt/scripting/ObjectTemplate/Properties/SetRotation.html"/>
        [PropertyName("setRotation")]
        public ObjectProperty<string> SetRotation;

        public ChildTemplate(string name, Token token) : base(name, token)
        {

        }
    }
}
