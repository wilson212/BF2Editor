﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine
{
    public static class StringExtensions
    {
        /// <summary>
        /// Determines whether the beginning of this string instance matches any 
        /// of the specified strings.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static bool StartsWithAny(this string text, params string[] items)
        {
            foreach (string item in items)
            {
                if (text.StartsWith(item))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Splits a string into substrings that are based on the characters in an array,
        /// while keeping double quoted text intact.
        /// </summary>
        /// <param name="splitChars">A character array that delimits the substrings in this string</param>
        /// <param name="removeEmptyEntries">Remove empty entries after spliting the strings?</param>
        /// <returns></returns>
        public static string[] SplitWithQuotes(this string text, char[] splitChars, bool removeEmptyEntries)
        {
            // Get our split options
            StringSplitOptions opts = (removeEmptyEntries) 
                ? StringSplitOptions.RemoveEmptyEntries 
                : StringSplitOptions.None;

            // Split the string by the quote character, creating an array
            // of parts. Even numbered indexes are outside of quotes, wheras
            // odd numbered indexes are located inside quotes.
            return text.Split('"').Select((element, index) =>
                {
                    // If even index, that we are outside of quotes
                    if (index % 2 == 0) 
                        // Split the item using the supplied characters
                        return element.Split(splitChars, opts);
                    else
                        // Keep the entire item, since it is in quotes
                        return new string[] { element };
                })
                .SelectMany(element => element)
                .ToArray();
        }
    }
}
