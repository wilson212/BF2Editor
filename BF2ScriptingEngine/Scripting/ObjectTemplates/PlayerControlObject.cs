using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting.Objects
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
    public class PlayerControlObject // : ObjectTemplate
    {
        public PlayerControlObject()
        { 

        }
    }
}
