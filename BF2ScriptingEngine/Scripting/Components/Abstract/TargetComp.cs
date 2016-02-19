using System;

namespace BF2ScriptingEngine.Scripting.Components
{
    public abstract class TargetComp : ObjectTemplate, IComponent
    {
        public TargetComp(string name, Token token) : base(name, token)
        {

        }
    }
}
