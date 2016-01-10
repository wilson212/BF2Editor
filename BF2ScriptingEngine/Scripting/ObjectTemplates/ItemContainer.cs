using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    public class ItemContainer : ObjectTemplate
    {
        /// <summary>
        /// 
        /// </summary>
        [PropertyName("addTemplate")]
        public ObjectProperty<List<string>> Adds;

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("replaceItem")]
        public ObjectProperty<List<string>> Replaces;

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("unlockLevel")]
        public ObjectProperty<int> UnlockLevel;

        public ItemContainer(string Name, Token Token) : base(Name, Token) { }
    }
}
