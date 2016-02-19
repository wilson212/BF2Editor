using System;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting.Components
{
    public abstract class ZoomComp : ConFileObject, IComponent
    {
        [PropertyName("startCameraId")]
        public virtual ObjectProperty<int> StartCameraId { get; internal set; }

        [PropertyName("startCameraViewMode")]
        public virtual ObjectProperty<int> StartCameraViewMode { get; internal set; }

        public ZoomComp(string name, Token token) : base(name, token)
        {

        }
    }
}
