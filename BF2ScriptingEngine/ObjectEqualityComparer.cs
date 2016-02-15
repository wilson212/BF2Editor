using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine
{
    /// <summary>
    /// This EqualityComparer is used to compare objects located in a 
    /// <see cref="Scope.Objects"/> tupple key
    /// </summary>
    /// <remarks>
    /// As far as I can tell at the moment, The object names are Case and Type sensetive!
    /// 
    /// Examples:
    ///     GeometryTemplate(us_kits) != GeometryTemplate(US_Kits)
    ///     AiTemplate(Ahz_AH1) != WeaponTemplate(Ahz_AH1)
    /// </remarks>
    internal class ObjectEqualityComparer : IEqualityComparer<Tuple<string, ReferenceType>>
    {
        /// <summary>
        /// Provide a consitant english culture to compare strings
        /// </summary>
        private static CultureInfo EnglishCulture = CultureInfo.CreateSpecificCulture("en-US");

        /// <summary>
        /// The <see cref="StringComparer"/> we will use to compare the object name
        /// in the <see cref="Tuple{T1, T2}"/>
        /// </summary>
        private static StringComparer Comparer = StringComparer.Create(EnglishCulture, false);

        /// <summary>
        /// Case-Insensetive comparison, to compare the Item2 in the Tupple
        /// </summary>
        private static StringComparer Comparer2 = StringComparer.Create(EnglishCulture, true);

        public bool Equals(Tuple<string, ReferenceType> x, Tuple<string, ReferenceType> y)
        {
            return Comparer.Equals(x.Item1, y.Item1) && Comparer2.Equals(x.Item2.Name, y.Item2.Name);
        }

        public int GetHashCode(Tuple<string, ReferenceType> tuple)
        {
            return Comparer.GetHashCode(tuple.Item1) ^ Comparer2.GetHashCode(tuple.Item2); //.GetHashCode();
        }
    }
}
