using System.Collections;
using System.Collections.Generic;

namespace CleanCollections
{
    internal struct CleanListEnumerator<T> : IEnumerator<T>
    {
        private readonly IList<T> _list;
        private T _current;
        private int _index;
        private Queue<ChunkedIndex>.Enumerator _deletedEnumerator;
        private bool _hasMoreDeletedItems;

        public CleanListEnumerator(IList<T> list, Queue<ChunkedIndex> deletedIndeces) : this()
        {
            _list = list;
            _deletedEnumerator = deletedIndeces.GetEnumerator();
            _hasMoreDeletedItems = _deletedEnumerator.MoveNext();
        }

        public void Dispose()
        {
            _deletedEnumerator.Dispose();
        }

        public bool MoveNext()
        {
            if (_index >= _list.Count)
            {
                return false;
            }

            // Skip any deleted items
            if (!SkipDeletedItems())
            {
                return false;
            }

            _current = _list[_index];

            _index++;
                
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>False if we read to the end of the list.</returns>
        private bool SkipDeletedItems()
        {
            while (_hasMoreDeletedItems && _deletedEnumerator.Current.AbsoluteIndex == _index)
            {
                // Skip this index as it is deleted
                _index++;
                _hasMoreDeletedItems = _deletedEnumerator.MoveNext();

                if (_index >= _list.Count)
                {
                    return false;
                }
            }

            return true;
        }

        public void Reset()
        {
            _index = -1;
        }

        public T Current { get { return _current; } private set { _current = value; } }

        object IEnumerator.Current { get { return Current; } }
    }
}