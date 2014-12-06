using System.Collections.Generic;

namespace CleanCollections.Tests
{
    static internal class ListExtensions
    {
        public static void AddThenRemove(this IList<int> list, int maxSize)
        {
            const int iterations = 10;
            for (var j = 0; j < iterations; j++)
            {
                for (var i = 0; i < maxSize; i++)
                {
                    list.Add(i);
                }

                for (var i = maxSize - 1; i >= 0; i--)
                {
                    list.RemoveAt(i);
                }
            }
        }
    }
}