using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting.GeometryTemplates
{
    public class SkinnedMesh : GeometryTemplate
    {
        /// <summary>
        /// Gets or Sets the LOD (Level Of Detail) of the model will be 
        /// seen at a particular distance.
        /// </summary>
        [PropertyName("setSubGeometryLodDistance"), IndexedList]
        public ObjectProperty<List<string[]>> SubGeometryLodDistances;




        public SkinnedMesh(string name, Token token) : base(name, token)
        {

        }
    }
}
