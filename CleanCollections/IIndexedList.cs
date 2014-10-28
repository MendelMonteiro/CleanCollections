using System.Collections.Generic;

namespace CleanCollections
{
    public interface IIndexedList<T> : IList<T>
    {
        /// <summary>
        /// Returns the index of the newly added item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        new int Add(T item);
    }
}