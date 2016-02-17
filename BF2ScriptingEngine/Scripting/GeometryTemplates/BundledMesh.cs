using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting
{
    public class BundledMesh : GeometryTemplate
    {
        [PropertyName("nrOfAnimatedUVMatrix")]
        public ObjectPropertyList<int> NumOfAnimatedUVMatrix { get; internal set; }

        [PropertyName("setMaterialReflectionScale")]
        public ObjectPropertyList<int, int, int, double> MaterialReflectionScale
        {
            get; internal set;
        }

        [PropertyName("setSpecularStaticGloss")]
        public ObjectPropertyList<int, int, int, int> SpecularStaticGloss
        {
            get; internal set;
        }

        [PropertyName("compressVertexData")]
        public ObjectPropertyList<bool> CompressVertexData { get; internal set; }

        [PropertyName("maxTextureRepeat")]
        public ObjectPropertyList<int> MaxTextureRepeat { get; internal set; }

        public BundledMesh(string name, Token token) : base(name, token)
        {

        }
    }
}
