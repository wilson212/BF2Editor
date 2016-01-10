using System;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// Tells the AI bots how to operate vehicles, and what they're worth.
    /// </summary>
    public abstract class AiTemplatePlugin : ConFileObject
    {
        public AiTemplatePlugin(string Name, Token Token)  : base(Name, "aiTemplatePlugIn", Token) { }

        /// <summary>
        /// Creates a new instance of AiTemplate with the following attributes
        /// </summary>
        /// <param name="Params">0 => ReferenceCall, 1 => ".Create", 2 => TemplateType, 3 => ObjectName</param>
        /// <param name="Token">The ConFile token</param>
        public static AiTemplatePlugin Create(string[] Params, Token Token)
        {
            // Switch to the object type
            switch (Params[2].ToLowerInvariant())
            {
                case "armament": return new Armament(Params[3], Token);
                case "mobile": return new Mobile(Params[3], Token);
                case "physical": return new Physical(Params[3], Token);
                case "cover": return new Cover(Params[3], Token);
                case "unit": return new Unit(Params[3], Token);
                case "controlinfo": return new ControlInfo(Params[3], Token);
                default:
                    throw new Exception("Invalid AiTemplatePlugIn object type \"" + Params[2] + "\"");
            }
        }
    }
}
