using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// This object represents a Run or Include statement found within 
    /// a <see cref="ConFile"/>.
    /// </summary>
    /// <example>
    /// - run object.tweak
    /// - include object.tweak arg1 arg2
    /// </example>
    public class RunStatement : Statement
    {
        /// <summary>
        /// Contains our list of spliting characters
        /// </summary>
        public static readonly char[] SplitChars = new char[] { ' ', '\t' };

        /// <summary>
        /// A quote
        /// </summary>
        private const string QUOTE = "\"";

        /// <summary>
        /// Gets the filename that this Run command will execute
        /// </summary>
        public string FileName { get; internal set; }

        /// <summary>
        /// Gets the arguments being passed to the File when parsed
        /// </summary>
        public string[] Arguments { get; internal set; }

        public RunStatement(Token token) : base(token)
        {
            // Load token args
            // Begin our array builder
            TokenArgs tokenArgs = new TokenArgs();
            List<string> args = new List<string>();

            // Split the line after the reference call into arguments
            string[] parts = token.Value.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);

            // Skip Run/Include
            parts = parts.Skip(1).ToArray();

            // Fix Quotes
            StringBuilder builder = new StringBuilder();
            bool inQuote = false;
            foreach (string part in parts)
            {
                if (!inQuote && part.StartsWith(QUOTE))
                {
                    if (part.EndsWith(QUOTE))
                    {
                        builder.Append($"{part}");

                        // Add the final quoted string as a single part
                        args.Add(builder.ToString());
                        builder.Clear();
                    }
                    else
                    {
                        inQuote = true;
                        builder.Append($"{part} ");
                    }
                }
                else if (inQuote && part.EndsWith(QUOTE))
                {
                    inQuote = false;
                    builder.Append($"{part}");

                    // Add the final quoted string as a single part
                    args.Add(builder.ToString());
                    builder.Clear();
                }
                else
                {
                    args.Add(part);
                }
            }

            // Set internals
            FileName = args[0];
            Arguments = args.Skip(1).ToArray();
        }
    }
}
