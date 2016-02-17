using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BF2ScriptingEngine.Scripting.Attributes;
using BF2ScriptingEngine.Scripting.Components;

namespace BF2ScriptingEngine.Scripting
{
    public class GenericFireArm : ObjectTemplate
    {
        /// <summary>
        /// The Fire Component
        /// </summary>
        [Component("MultiFireComp", "SingleFireComp")]
        [PropertyName("fire")]
        [Comment(
            Before = "---BeginComp:FireComp ---"
        //After = "---EndComp ---"
        )]
        public ObjectProperty<FireComp> FireComp { get; set; }

        /// <summary>
        /// The Ammo Component
        /// </summary>
        [Component("DefaultAmmoComp")]
        [PropertyName("ammo")]
        [Comment(
            Before = "---BeginComp:DefaultAmmoComp ---"
        //After = "---EndComp ---"
        )]
        public ObjectProperty<DefaultAmmoComp> AmmoComp { get; set; }

        public GenericFireArm(string Name, Token Token) : base(Name, Token) { }
    }
}
