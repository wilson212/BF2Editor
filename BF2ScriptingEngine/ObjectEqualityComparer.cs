using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine
{
    /// <summary>
    /// This EqualityComparer is used to compare the <see cref="ObjectManager.Globals"/> tupple key,
    /// by ignoring the Tupple string character case.
    /// </summary>
    internal class ObjectEqualityComparer : IEqualityComparer<Tuple<string, ObjectType>>
    {
        private static CultureInfo EnglishCulture = CultureInfo.CreateSpecificCulture("en-US");

        /// <summary>
        /// The <see cref="StringComparer"/> we will use to compare the object name
        /// in the <see cref="Tuple{T1, T2}"/>
        /// </summary>
        private static StringComparer Comparer = StringComparer.Create(EnglishCulture, false);

        public bool Equals(Tuple<string, ObjectType> x, Tuple<string, ObjectType> y)
        {
            return Comparer.Equals(x.Item1, y.Item1) && x.Item2 == y.Item2;
        }

        public int GetHashCode(Tuple<string, ObjectType> tuple)
        {
            return Comparer.GetHashCode(tuple.Item1) ^ tuple.Item2.GetHashCode();
        }
    }
}
