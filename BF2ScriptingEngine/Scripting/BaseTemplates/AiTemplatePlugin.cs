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
        /// <param name="tokenArgs">The command line token</param>
        /// <param name="Token">The ConFile token</param>
        public static AiTemplatePlugin Create(TokenArgs tokenArgs, Token Token)
        {
            string type = tokenArgs.Arguments[0];
            string name = tokenArgs.Arguments[1];

            // Switch to the object type
            switch (type.ToLowerInvariant())
            {
                case "armament": return new Armament(name, Token);
                case "mobile": return new Mobile(name, Token);
                case "physical": return new Physical(name, Token);
                case "cover": return new Cover(name, Token);
                case "unit": return new Unit(name, Token);
                case "controlinfo": return new ControlInfo(name, Token);
                default:
                    throw new Exception("Invalid AiTemplatePlugIn object type \"" + type + "\"");
            }
        }
    }
}
