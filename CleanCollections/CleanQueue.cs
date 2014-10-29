using System;
using System.Collections;
using System.Collections.Generic;

namespace CleanCollections
{
    public class CleanQueue<T> : IEnumerable<T>
    {
        private readonly int _blockSize;
        private readonly T[][] _subArrays;
        private int _count;
        private int _capacity;

        public CleanQueue(int maxSize, int blockSize)
        {
            _blockSize = blockSize;

            _subArrays = new T[GetChunkIndex(maxSize, blockSize)][];
        }

        private static short GetChunkIndex(int index, int blockSize)
        {
            return (short)Math.Floor((double)index / blockSize);
        }

        public void Enqueue(T item)
        {
            EnsureCapacity();

            int count = _count;
            int blockSize = _blockSize;
            short chunkIndex = GetChunkIndex(count, blockSize);
            int localIndex = count - (chunkIndex * blockSize);

            _subArrays[chunkIndex][localIndex] = item;
            _count++;
        }

        private void EnsureCapacity()
        {
            if (_count >= _capacity)
            {
                int blockSize = _blockSize;
                short chunkIndex = GetChunkIndex(_count, blockSize);
                _subArrays[chunkIndex] = new T[blockSize];
                _capacity += blockSize;
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
            int blockSize = _blockSize;
            short chunkIndex = GetChunkIndex(index, _blockSize);
            int localIndex = index - (chunkIndex * blockSize);
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
