using System;
using System.Collections;
using System.Collections.Generic;

namespace CleanCollections
{
    /// <summary>
    /// A queue that grows (by blockSize) without creating any garbage.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CleanQueue<T> : IEnumerable<T>
    {
        private readonly int _blockSize;
        private readonly T[][] _subArrays;
        private int _count;
        private int _capacity;
        private readonly int _blockPowerOfTwo;

        public CleanQueue(int maxSize, int blockSize)
        {
            if (!Util.IsPowerOfTwo(blockSize)) throw new ArgumentException("blockSize must be a power of two");

            _blockPowerOfTwo = (int)Math.Log(blockSize, 2);

            _blockSize = blockSize;

            _subArrays = new T[(short)(maxSize / blockSize)][];
        }

        public void Enqueue(T item)
        {
            EnsureCapacity();

            short chunkIndex = (short)(_count >> _blockPowerOfTwo);
            int localIndex = _count - (chunkIndex * _blockSize);

            _subArrays[chunkIndex][localIndex] = item;
            _count++;
        }

        private void EnsureCapacity()
        {
            if (_count >= _capacity)
            {
                short chunkIndex = (short)(_count >> _blockPowerOfTwo);
                _subArrays[chunkIndex] = new T[_blockSize];
                _capacity += _blockSize;
            }
        }

        public T Dequeue()
        {
            var item = GetItem(_count - 1);

            // Set current item to default(T) ?
            _count--;
            return item;
        }

        private T GetItem(int index)
        {
            short chunkIndex = (short)(index >> _blockPowerOfTwo);
            int localIndex = index - (chunkIndex * _blockSize);
            var item = _subArrays[chunkIndex][localIndex];
            return item;
        }

        public void Clear()
        {
            _count = 0;
        }

        public int Count { get { return _count; } private set { _count = value; } }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public struct Enumerator : IEnumerator<T>
        {
            private readonly CleanQueue<T> _queue;
            private T _current;
            private int _index;

            public Enumerator(CleanQueue<T> queue)
            {
                _index = -1;
                _queue = queue;
                _current = default(T);
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                bool beforeEnd = _index + 1 < _queue.Count;

                if (beforeEnd)
                {
                    _index++;
                    _current = _queue.GetItem(_index);
                }

                return beforeEnd;
            }

            public void Reset()
            {
                _index = -1;
            }

            public T Current { get { return _current; } private set { _current = value; } }

            object IEnumerator.Current { get { return Current; } }
        }
    }
}
