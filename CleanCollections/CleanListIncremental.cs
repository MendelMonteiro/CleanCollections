using System;
using System.Collections;
using System.Collections.Generic;

namespace CleanCollections
{
    public class CleanListIncremental<T> : IIndexedList<T>
    {
        private readonly int _blockSize;
        private readonly T[][] _subArrays;
        private int _count;
        private readonly Queue<ChunkedIndex> _deletedIndeces = new Queue<ChunkedIndex>();
        private int _capacity;
        private int _lastChunk = -1;
        private readonly int _blockPowerOfTwo;

        public CleanListIncremental(int maxSize, int blockSize)
        {
            double log = Math.Log(blockSize, 2);
            double logNoInt = (int) log;
            if (Math.Abs((log - logNoInt)) > 0) throw new ArgumentException("blockSize must be a power of two");

            _blockPowerOfTwo = (int) log;

            _blockSize = blockSize;
            _subArrays = new T[(int) Math.Ceiling((double) maxSize/blockSize)][];
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new CleanListEnumerator<T>(this, _deletedIndeces);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        int IIndexedList<T>.Add(T item)
        {
            var count = _count;

            EnsureCapacity();

            int chunkIndex, localIndex;
            GetChunkedIndex(count, out chunkIndex, out localIndex);

            _subArrays[chunkIndex][localIndex] = item;
            _count = count + 1;
            return _count;
        }

        private void EnsureCapacity()
        {
            if (_count >= _capacity)
            {
                Grow(_blockSize);
            }
        }

        private void Grow(int blockSize)
        {
            _lastChunk++;
            _subArrays[_lastChunk] = new T[blockSize];
            _capacity += blockSize;
        }

        private ChunkedIndex GetChunkedIndex2(int index)
        {
            var chunkIndex = index >> _blockPowerOfTwo;
            var localIndex = index - (chunkIndex << _blockPowerOfTwo);
//            var chunkIndex = index / _blockSize;
//            var localIndex = index - (chunkIndex * _blockSize);
            return new ChunkedIndex(chunkIndex, localIndex, index);
        }

        private void GetChunkedIndex(int index, out int chunkIndex, out int localIndex)
        {
            chunkIndex = index >> _blockPowerOfTwo;
            localIndex = index - (chunkIndex << _blockPowerOfTwo);
//            chunkIndex = index / _blockSize;
//            localIndex = index - (chunkIndex * _blockSize);
        }

        public void Add(T item)
        {
            ((IIndexedList<T>) this).Add(item);
        }

        public void Clear()
        {
            _count = 0;
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public int Count { get { return _count; } private set { _count = value; } }

        public bool IsReadOnly { get; private set; }

        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            int chunkIndex, localIndex;
            GetChunkedIndex(index, out chunkIndex, out localIndex);
            _deletedIndeces.Enqueue(new ChunkedIndex(chunkIndex, localIndex, index));
            _count--;
        }

        private void CheckIndex(int index)
        {
            if (index < (uint)0 || (uint)index > (uint)Count - 1) throw new IndexOutOfRangeException();
        }

        public T this[int index]
        {
            get
            {
                CheckIndex(index);
                int chunkIndex, localIndex;
                GetChunkedIndex(index, out chunkIndex, out localIndex);
                return _subArrays[chunkIndex][localIndex];
            }
            set
            {
                CheckIndex(index);
                int chunkIndex, localIndex;
                GetChunkedIndex(index, out chunkIndex, out localIndex);
                _subArrays[chunkIndex][localIndex] = value;
            }
        }
    }
}