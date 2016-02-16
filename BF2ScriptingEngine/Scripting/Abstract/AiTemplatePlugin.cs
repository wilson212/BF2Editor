using System;
using System.Collections.Generic;
using System.Linq;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// Tells the AI bots how to operate vehicles, and what they're worth.
    /// </summary>
    public abstract class AiTemplatePlugin : ConFileObject
    {
        #region Mappings

        /// <summary>
        /// Contains a Mapping of object types, that derive from <see cref="AiTemplatePlugin"/>
        /// </summary>
        public static Dictionary<string, Type> ObjectTypes { get; set; }

        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="AiTemplatePlugin"/>
        /// </summary>
        /// <param name="Name">The unique name of the plugin</param>
        /// <param name="Token">The token that represents this plugin</param>
        public AiTemplatePlugin(string Name, Token Token)  : base(Name, Token) { }

        static AiTemplatePlugin()
        {
            // Create object mappings
            var Comparer = StringComparer.InvariantCultureIgnoreCase;
            Type baseType = typeof(AiTemplatePlugin);
            Type[] typelist = TypeCache.GetTypesInNamespace("BF2ScriptingEngine.Scripting")
                .Where(x => baseType.IsAssignableFrom(x)).ToArray();
            ObjectTypes = typelist.ToDictionary(x => x.Name, v => v, Comparer);
        }

        /// <summary>
        /// Creates a new instance of AiTemplateplugIn with the following attributes
        /// </summary>
        /// <param name="tokenArgs">The command line token</param>
        /// <param name="token">The ConFile token</param>
        public static ConFileObject Create(Token token)
        {
            // Make sure we have the correct number of arguments
            if (token.TokenArgs.Arguments.Length != 2)
            {
                throw new ArgumentException(String.Concat(
                    "Invalid arguments count for AiTemplatePlugIn;",
                        $"Got {token.TokenArgs.Arguments.Length}, Expecting 2."
                ));
            }

            // Extract our arguments
            string type = token.TokenArgs.Arguments[0];
            string name = token.TokenArgs.Arguments[1];

            // Ensure this type is supported
            if (!ObjectTypes.ContainsKey(type))
                throw new ParseException("Invalid AiTemplatePlugIn derived type \"" + type + "\".", token);

            // Create and return our object instance
            var t = ObjectTypes[type];
            return (ConFileObject)Activator.CreateInstance(t, name, token);
        }
    }
}
