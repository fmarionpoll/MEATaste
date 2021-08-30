using System;
using System.Collections.Generic;
using System.Linq;

namespace MEATaste.Infrastructure
{
    public static class EnumerableExtensions
    {
        public static void Iter<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable) action(item);
        }

        public static int IndexOf<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            var array = enumerable.ToArray();

            for (var i = 0; i < array.Count(); i++)
            {
                if (predicate(array[i]))
                    return i;
            }

            return -1;
        }
    }
}
