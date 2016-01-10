﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// A token is a reference to a parsable object found within a con/Ai file.
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
        public ConFile File { get; protected set; }

        /// <summary>
        /// Gets the position this information is found at in the original source
        /// </summary>
        /// <remarks>The line number</remarks>
        public int Position { get; protected set; }

        /// <summary>
        /// Gets the representation of the kind of instruction we're working with
        /// </summary>
        public TokenType Kind { get; protected set; }

        /// <summary>
        /// Gets the regular expression match for this token
        /// </summary>
        public Match RegexMatch { get; protected set; }

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
                RegexMatch = match,
                Value = match.Value
            };
        }
    }
}