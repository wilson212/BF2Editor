using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    public class KitTemplate
    {
        public static KitTemplate Create(string Type, string Name)
        {
            return new KitTemplate();
        }
    }
}
