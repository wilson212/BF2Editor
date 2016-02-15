using System;
using System.Collections.Generic;
using System.Text;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// This object is what Battlefield 2 uses as a base object for vehicles and 
    /// stationary weapons, or whatever the player can "enter". The PlayerControlObject 
    /// itself contain information like Hitpoints and damage information, 
    /// ammo/weapon icons, and other miscellaneous properties.
    /// </summary>
    /// <remarks>
    /// Most of the actual behavior for vehicles and stationary weapons is determined by the child objects. 
    /// In simple vehicles (like stationary weapons), most of the objects are direct children, but in complex 
    /// vehicles (tanks, jeeps, planes, ships) the only direct children of a PlayerControlObject are FloatingBundle t
    /// ype objects (for ships, amphibious vehicles, and submarines) and a LodObject that contains a Bundle with 
    /// all of the physics, weapons, and other PlayerControlObjects (for multiple positions).
    /// </remarks>
    /// <seealso cref="http://bfmods.com/mdt/scripting/ObjectTemplate/Types/PlayerControlObject.html"/>
    /// <example>The PlayerControlObject object type is created by ObjectTemplate.Create.</example>
    public class PlayerControlObject : ObjectTemplate
    {
        [PropertyName("collisionMesh")]
        public ObjectProperty<string> CollisionMesh { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// First Param is the meterial index of the mesh
        /// Second Param is The material name
        /// 3rd Param is material number
        /// </remarks>
        [PropertyName("mapMaterial")]
        public ObjectPropertyList<int, string, int> Materials { get; internal set; }

        public PlayerControlObject(string Name, Token Token) : base(Name, Token)
        { 

        }
    }
}
