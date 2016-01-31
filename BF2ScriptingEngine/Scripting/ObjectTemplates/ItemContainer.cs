using System;
using System.Collections.Generic;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting
{
    public class ItemContainer : ObjectTemplate
    {
        /// <summary>
        /// Contains a list of child objects attached to this object
        /// </summary>
        /// <remarks>
        /// Any SetPosition and SetRotation commands following this will 
        /// apply to the attached object, until another AddTemplate command 
        /// is encountered.
        /// </remarks>
        [Comment(Before = "", After = "")] // Override base comments
        public override ObjectProperty<List<ChildTemplate>> Templates { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("replaceItem")]
        public ObjectProperty<List<string>> Replaces { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("unlockLevel")]
        public ObjectProperty<int> UnlockLevel { get; set; }

        public ItemContainer(string Name, Token Token) : base(Name, Token) { }
    }
}
