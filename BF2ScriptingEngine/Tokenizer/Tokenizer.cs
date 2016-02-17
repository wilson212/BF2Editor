using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// The Tokenizer class is responsible for taking an indexed collection of
    /// lines from a con file, and breaking down each line into a recognizable 
    /// element token. These tokens are used to direct the script engine into
    /// which action it should take when it runs into each line.
    /// </summary>
    public static class Tokenizer
    {
        /// <summary>
        /// Breaks an array of indexed strings into recognizable tokens
        /// </summary>
        /// <param name="File">The ConFile object where these lines are located</param>
        /// <param name="Lines">The indexed input strings to break up into tokens</param>
        /// <returns>The set of tokens located within the string. 
        /// <paramref name="Lines"/> is updated as a result of this call, and will contain entries that did not
        /// match any of the token expressions</returns>
        public static Token[] Tokenize(ConFile File, ref Dictionary<int, string> Lines)
        {
            List<Token> tokens = new List<Token>();

            // Tokenize and add each token to the list of matched rules
            foreach (KeyValuePair<TokenType, string> token in TokenExpressions)
            {
                tokens.AddRange(Tokenize(token.Key, token.Value, File, ref Lines));
            }

            return tokens.OrderBy(x => x.Position).ToArray();
        }

        /// <summary>
        /// Performs tokenization on a non-tokenized input string with the specified patterns
        /// </summary>
        /// <param name="input">The input string we are trying to tokenize</param>
        /// <exception cref="ArgumentException">
        /// Thrown in the input string cannot be tokenized (unrecognized input)
        /// </exception>
        public static Token Tokenize(string input)
        {
            Regex regex;

            // Tokenize and add each token to the list of matched rules
            foreach (KeyValuePair<TokenType, string> token in TokenExpressions)
            {
                // Create our regex and remove excess whitespace on our input
                regex = new Regex(token.Value, RegexOptions.Multiline | RegexOptions.IgnoreCase);
                string line = input.Trim();

                // Check to see if we have a match
                Match match = regex.Match(line);
                if (match.Success)
                {
                    return Token.Create(token.Key, match, null, 0);
                }
            }

            // We will never get here using ScriptEngine tokens!
            throw new ArgumentException($"Unrecognized input \"{input}\"");
        }

        /// <summary>
        /// Performs tokenization of a collection of non-tokenized data parts with a specific pattern
        /// </summary>
        /// <param name="tokenKind">The name to give the located tokens</param>
        /// <param name="pattern">The pattern to use to match the tokens</param>
        /// <param name="untokenizedParts">The portions of the input that have yet to be tokenized (organized as position vs. text in source)</param>
        /// <returns>
        ///     The set of tokens matching the given pattern located in the untokenized portions of the input, 
        ///     <paramref name="untokenizedParts"/> is updated as a result of this call
        /// </returns>
        private static IEnumerable<Token> Tokenize(
            TokenType tokenKind, 
            string pattern, 
            ConFile file, 
            ref Dictionary<int, string> untokenizedParts)
        {
            // Do a bit of setup
            var unMatchedParts = new Dictionary<int, string>();
            var resultTokens = new List<Token>();
            var regex = new Regex(pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);

            // Look through all of our currently untokenized data
            foreach (KeyValuePair<int, string> part in untokenizedParts)
            {
                // Trim our line, and remove empty ones
                string line = part.Value.Trim();
                //if (line.Length == 0) continue;

                // Check to see if we have a match
                Match match = regex.Match(line);

                // If we don't have any, keep the data as untokenized and move to the next chunk
                if (!match.Success)
                {
                    unMatchedParts.Add(part.Key, line);
                    continue;
                }

                // Store the untokenized data in a working copy and save the absolute index it reported itself at in the source file
                resultTokens.Add(Token.Create(tokenKind, match, file, part.Key)); 
            }

            // Update the untokenized data to contain what we couldn't process with this pattern
            untokenizedParts = unMatchedParts;

            // Return the tokens we were able to extract
            return resultTokens;
        }

        /// <summary>
        /// An array of all the expressions needed to tokenize the contents
        /// of a Con file.
        /// </summary>
        /// <remarks>
        /// Order is important here, as we itterate through these expressions, lines
        /// that match an expression are removed from our source. As we get to the bottom
        /// of this array, the source will get smaller and smaller as we match our expressions.
        /// </remarks>
        internal static KeyValuePair<TokenType, string>[] TokenExpressions = new[]
        {
            // === Parse comments first, as somethings like objects and properties can be commented out === //

            // Multiline Rem Comment on a single line
            new KeyValuePair<TokenType, string>(TokenType.RemComment,
                @"^beginRem(?<value>.*?)endRem$"
            ),

            // Multiline Rem Comment START
            new KeyValuePair<TokenType, string>(TokenType.BeginRem,
                @"^beginRem(?<value>.*?)$"
            ),

            // Multiline Rem Comment END
            new KeyValuePair<TokenType, string>(TokenType.EndRem,
                @"^(?<value>.*)endRem$"
            ),

            // Single line Rem Comment
            new KeyValuePair<TokenType, string>(TokenType.RemComment,
                @"^rem([\s|\t]+)(?<value>.*)?$"
            ),

            // === Objects === //

            new KeyValuePair<TokenType, string>(TokenType.ActiveSwitch,
                @"^(?<reference>[a-z]+)\.active(?<type>[a-z_]*)([\s|\t]+)(?<value>.*)?$"
            ),

            // Object property value, HAS TO FOLLOW everything else with a similar expression
            // due to the ungreediness of this regular expression
            new KeyValuePair<TokenType, string>(TokenType.ObjectProperty,
                @"^(?<reference>[a-z]+)\.(?<property>[a-z_0-9\.]+)([\s|\t]+)(?<value>.*)?$"
            ),

            // === Vars Conditionals === //

            // If statements
            new KeyValuePair<TokenType, string>(TokenType.IfStart, @"^if(?<value>.*)?$"),

            // End If
            new KeyValuePair<TokenType, string>(TokenType.EndIf, @"^(?<value>.*)endIf$"),

            // Variable
            new KeyValuePair<TokenType, string>(TokenType.Variable, 
                @"^(?:var[\s\t]+)(?<name>[a-z0-9_]+)[\s\t]*(?:=[\s\t]*)?(?<value>.*?)?$"
            ),
            new KeyValuePair<TokenType, string>(TokenType.Variable, 
                @"^(?<name>[a-z0-9_]+)[\s\t]*(?:=[\s\t]*)(?<value>.*?)$"
            ),

            // Constant
            new KeyValuePair<TokenType, string>(TokenType.Constant,
                @"^(?:const[\s\t]+)(?<name>[a-z0-9_]+)[\s\t]*(?:=[\s\t]*)(?<value>.*?)$"
            ),

            // Include command
            new KeyValuePair<TokenType, string>(TokenType.Include, @"^include(?<value>.*)?$"),

            // Run command
            new KeyValuePair<TokenType, string>(TokenType.Run, @"^run(?<value>.*)?$"),

            // Finally, match everything else
            new KeyValuePair<TokenType, string>(TokenType.None, @"^(?<value>.*?)$")
        };
    }
}
