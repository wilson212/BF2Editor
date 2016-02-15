using System;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// Tells the AI bots how to operate vehicles, and what they're worth.
    /// </summary>
    public abstract class AiTemplatePlugin : ConFileObject
    {
        public AiTemplatePlugin(string Name, Token Token)  : base(Name, Token) { }

        /// <summary>
        /// Creates a new instance of AiTemplate with the following attributes
        /// </summary>
        /// <param name="tokenArgs">The command line token</param>
        /// <param name="token">The ConFile token</param>
        public static AiTemplatePlugin Create(Token token)
        {
            string type = token.TokenArgs.Arguments[0];
            string name = token.TokenArgs.Arguments[1];

            // Switch to the object type
            switch (type.ToLowerInvariant())
            {
                case "armament": return new Armament(name, token);
                case "mobile": return new Mobile(name, token);
                case "physical": return new Physical(name, token);
                case "cover": return new Cover(name, token);
                case "unit": return new Unit(name, token);
                case "controlinfo": return new ControlInfo(name, token);
                default:
                    throw new Exception("Invalid AiTemplatePlugIn object type \"" + type + "\"");
            }
        }
    }
}
