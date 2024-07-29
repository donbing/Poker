using System.Collections.Generic;

namespace PokerPolker
{
    internal static class EnumerableExtensions
    {
        public static IList<T> ShiftLeft<T>(this IList<T> source, int amount)
        {
            var demo = new List<T>();
            for (var i = 0; i < source.Count; i++)
            {
                var v = i + amount;
                var v1 = v % source.Count;
                demo.Insert(i, source[v1]);
            }
            return demo;
        }
    }
}