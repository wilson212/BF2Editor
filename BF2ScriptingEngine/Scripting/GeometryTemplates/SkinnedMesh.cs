using System;
using System.Collections.Generic;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting.GeometryTemplates
{
    public class SkinnedMesh : GeometryTemplate
    {
        /// <summary>
        /// Defines the Kit index?
        /// </summary>
        [PropertyName("kit")]
        public ObjectProperty<int> Kit { get; set; }

        /// <summary>
        /// Drop Geometry?
        /// </summary>
        [PropertyName("dropGeom")]
        public ObjectProperty<int> DropGeom { get; set; }

        /// <summary>
        /// Gets or Sets the LOD (Level Of Detail) of the model will be 
        /// seen at a particular distance.
        /// </summary>
        [PropertyName("setSubGeometryLodDistance"), IndexedList]
        public ObjectProperty<List<string>> SubGeometryLodDistances { get; set; }

        public SkinnedMesh(string name, Token token) : base(name, token)
        {

        }
    }
}
