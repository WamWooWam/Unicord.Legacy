using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Controls
{
    internal static class Extensions
    {
        public static int FindIndex<T>(this List<T> list, Predicate<T> match)
        {
            return FindIndex(list, 0, list.Count, match);
        }

        public static int FindIndex<T>(this List<T> list, int startIndex, int count, Predicate<T> match)
        {
            if ((uint)startIndex > (uint)list.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (count < 0 || startIndex > list.Count - count)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (match == null)
            {
                throw new ArgumentNullException();
            }

            int endIndex = startIndex + count;
            for (int i = startIndex; i < endIndex; i++)
            {
                if (match(list[i])) return i;
            }
            return -1;
        }
 
    }
}
