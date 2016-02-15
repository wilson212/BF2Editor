using System;
using System.Collections.Generic;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting
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

        public SkinnedMesh(string name, Token token) : base(name, token)
        {

        }
    }
}
