using System;

namespace System
{
    static class IntExtensions
    {
        /// <summary>
        /// Returns whether this Int32 value is within the range of the specified values
        /// </summary>
        /// <param name="LowValue">The low end of the scale</param>
        /// <param name="HighValue">The high end of the scale</param>
        /// <returns></returns>
        public static bool InRange(this int input, int LowValue, int HighValue)
        {
            return (input >= LowValue && input <= HighValue);
        }
    }
}
