using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Performs an action of each element in an array
        /// </summary>
        /// <remarks>Credits to Alex Burtsev @ StackOverflow</remarks>
        /// <seealso cref="https://github.com/Burtsev-Alexey/net-object-deep-copy"/>
        public static void ForEach(this Array array, Action<Array, int[]> action)
        {
            if (array.LongLength == 0)
                return;

            ArrayTraverse walker = new ArrayTraverse(array);
            do action(array, walker.Position);
            while (walker.Step());
        }
    }

    internal class ArrayTraverse
    {
        public int[] Position;
        private int[] maxLengths;

        public ArrayTraverse(Array array)
        {
            maxLengths = new int[array.Rank];
            for (int i = 0; i < array.Rank; ++i)
            {
                maxLengths[i] = array.GetLength(i) - 1;
            }
            Position = new int[array.Rank];
        }

        public bool Step()
        {
            for (int i = 0; i < Position.Length; ++i)
            {
                if (Position[i] < maxLengths[i])
                {
                    Position[i]++;
                    for (int j = 0; j < i; j++)
                    {
                        Position[j] = 0;
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
