using System;
using System.Collections.Generic;

namespace Exploratorium.Extras
{
    public static class LinqExtensions
    {
        public static void ForEach<T>(this T[] array, Action<T> action)
        {
            if (array == null)
                throw new ArgumentNullException(nameof (array));
            if (action == null)
                throw new ArgumentNullException(nameof (action));
            for (int index = 0; index < array.Length; ++index)
                action(array[index]);
        }
        
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            if (items == null)
                throw new ArgumentNullException(nameof (items));
            if (action == null)
                throw new ArgumentNullException(nameof (action));
            foreach (var item in items) 
                action(item);
        }
    }
}