using System;
using System.Collections.Generic;
using System.Globalization;
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
        /// Determines whether the beginning of this string instance matches any 
        /// of the specified strings.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static bool StartsWithAny(this string text, StringComparison comparer, params string[] items)
        {
            foreach (string item in items)
            {
                if (text.StartsWith(item, comparer))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the end of this string instance matches any 
        /// of the specified strings.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static bool EndsWithAny(this string text, params string[] items)
        {
            foreach (string item in items)
            {
                if (text.EndsWith(item))
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

        /// <summary>
        /// Returns true if string is numeric and not empty or null or whitespace.
        /// Determines if string is numeric by parsing as Double
        /// </summary>
        /// <param name="str"></param>
        /// <param name="style">
        /// Optional style - defaults to NumberStyles.Number 
        /// (leading and trailing whitespace, leading and trailing sign, 
        /// decimal point and thousands separator) 
        /// </param>
        /// <param name="culture">Optional CultureInfo - defaults to InvariantCulture</param>
        /// <returns></returns>
        public static bool IsNumeric(this string str, NumberStyles style = NumberStyles.Number,
            CultureInfo culture = null)
        {
            double num;
            if (culture == null) culture = CultureInfo.InvariantCulture;
            return Double.TryParse(str, style, culture, out num) && !String.IsNullOrWhiteSpace(str);
        }
    }
}
