//   Copyright 2014 Mendel Monteiro-Beckerman
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System.Collections;
using System.Collections.Generic;

namespace CleanCollections
{
    /// <summary>
    /// A common enumerator for all the CleanList implementations (skips over any deleted indeces)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct CleanListEnumerator<T> : IEnumerator<T>
    {
        private readonly IList<T> _list;
        private T _current;
        private int _index;
        private CleanStack<ChunkedIndex>.Enumerator _deletedEnumerator;
        private bool _hasMoreDeletedItems;

        internal CleanListEnumerator(IList<T> list, CleanStack<ChunkedIndex> deletedIndeces) : this()
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