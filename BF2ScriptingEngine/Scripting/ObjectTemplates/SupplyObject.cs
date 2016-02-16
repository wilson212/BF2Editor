using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// This is the object used to heal, repair, and resupply players and vehicles. 
    /// It can be set specifically for ammunition, repairing, and healing for each 
    /// vehicle and ammuntion type
    /// </summary>
    public class SupplyObject : ObjectTemplate
    {
        public SupplyObject(string name, Token token) : base(name, token)
        {

        }
    }
}
