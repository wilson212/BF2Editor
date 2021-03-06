﻿using System;
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
        /// Do not let outsiders create tokens!!
        /// </summary>
        internal Token() { }

        /// <summary>
        /// Creates a new Token
        /// </summary>
        /// <param name="kind">The kind of this token</param>
        /// <param name="match">The Match the token is to be generated from</param>
        /// <param name="file">The con file this token is generating from</param>
        /// <param name="index">The line number this match is found on</param>
        /// <returns>The newly created token</returns>
        internal static Token Create(TokenType kind, Match match, ConFile file, int index)
        {
            Token token = new Token()
            {
                File = file,
                Position = match.Index + index,
                Kind = kind,
                Match = match,
                Value = match.Value
            };

            // We only create token args for object property types
            if (token.Kind == TokenType.ObjectProperty || token.Kind == TokenType.ActiveSwitch)
                SetTokenArgs(token);

            return token;
        }

        /// <summary>
        /// Converts the value of a <see cref="Token"/> into an array of parameters.
        /// Any values that are qouted will remain intact
        /// </summary>
        /// <param name="tokenValue">The value of the token</param>
        /// <returns></returns>
        internal static void SetTokenArgs(Token token)
        {
            // Create instance
            token.TokenArgs = new TokenArgs();

            // Break the line into {0 => Template name, 1 => The rest of the line}
            // We only split into 2 strings, because some values have dots
            string[] temp = token.Value.Split(new char[] { '.' }, 2);
            token.TokenArgs.ReferenceType = ReferenceManager.GetReferenceType(temp[0]);

            // Check for null
            if (token.TokenArgs.ReferenceType == null)
                throw new Exception($"Reference call to '{temp[0]}' is not supported");

            // Split the line after the reference call into arguments
            string[] parts = temp[1].SplitWithQuotes(ScriptEngine.SplitChars, true);
            token.TokenArgs.PropertyName = parts[0];

            // Type correction
            if (token.TokenArgs.ReferenceType.Mappings.ContainsKey(token.TokenArgs.PropertyName))
                token.Kind = TokenType.ObjectStart;

            // Skip the property/function name
            token.TokenArgs.Arguments = parts.Skip(1).ToArray();
        }
    }
}
