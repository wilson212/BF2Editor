using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// A token describes a parsable line(s) found within a con/Ai file.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Gets the con file where this token is taken from
        /// </summary>
        /// <remarks>
        /// Since some objects can be activated in other files, and have
        /// their properties set elsewhere, we also store the ConFile where
        /// this token is found in, so we can properly put this token and
        /// its value in the correct con file when saved
        /// </remarks>
        public ConFile File { get; internal set; }

        /// <summary>
        /// Gets the position this information is found at in the original source
        /// </summary>
        /// <remarks>The line number</remarks>
        public int Position { get; internal set; }

        /// <summary>
        /// Gets the representation of the kind of instruction we're working with
        /// </summary>
        public TokenType Kind { get; internal set; }

        /// <summary>
        /// Gets the <see cref="System.Text.RegularExpressions.Match"/> for this token
        /// </summary>
        public Match Match { get; internal set; }

        /// <summary>
        /// Gets this Token object as an <see cref="Scripting.TokenArgs"/> object ONLY IF this 
        /// <see cref="TokenType"/> is a <see cref="TokenType.ObjectStart"/> or a 
        /// <see cref="TokenType.ObjectProperty"/>.
        /// </summary>
        public TokenArgs TokenArgs { get; set; }

        /// <summary>
        /// If a comment was attached to this Token, it is stored here
        /// </summary>
        public RemComment Comment { get; set; }

        /// <summary>
        /// Gets or sets the value of the token
        /// </summary>
        /// <remarks>
        /// The value of this property will be the entire line found in the con file
        /// </remarks>
        public string Value { get; set; }

        /// <summary>
        /// Creates a new Token
        /// </summary>
        /// <param name="kind">The kind of this token</param>
        /// <param name="match">The Match the token is to be generated from</param>
        /// <param name="file">The con file this token is generating from</param>
        /// <param name="index">The line number this match is found on</param>
        /// <returns>The newly created token</returns>
        public static Token Create(TokenType kind, Match match, ConFile file, int index)
        {
            return new Token
            {
                File = file,
                Position = match.Index + index,
                Kind = kind,
                Match = match,
                Value = match.Value
            };
        }

        public static Token Create(TokenType kind, TokenArgs args, ConFile file)
        {
            return new Token
            {
                File = file,
                Position = 0,
                Kind = kind,
                TokenArgs = args,
                Match = Match.Empty,
                Value = args.ToString()
            };
        }
    }
}
