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
        private readonly CleanQueue<ChunkedIndex> _deletedIndeces;
        private int _capacity;
        private int _lastChunk = -1;
        private readonly int _blockPowerOfTwo;

        public CleanListIncremental(int maxSize, int blockSize)
        {
            if (!Util.IsPowerOfTwo(blockSize)) throw new ArgumentException("blockSize must be a power of two");

            _deletedIndeces = new CleanQueue<ChunkedIndex>(maxSize, blockSize);

            double log = Math.Log(blockSize, 2);
            _blockPowerOfTwo = (int)log;

            _blockSize = blockSize;
            _subArrays = new T[(int) Math.Ceiling((double) maxSize/blockSize)][];
        }

        public CleanListEnumerator<T> GetEnumerator()
        {
            return new CleanListEnumerator<T>(this, _deletedIndeces);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        int IIndexedList<T>.Add(T item)
        {
            var count = _count;

            EnsureCapacity();

            short chunkIndex;
            int localIndex;
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

        private void GetChunkedIndex(int index, out short chunkIndex, out int localIndex)
        {
//            chunkIndex = index / _blockSize;
//            localIndex = index - (chunkIndex * _blockSize);
            chunkIndex = (short)(index >> _blockPowerOfTwo);
            localIndex = index - (chunkIndex << _blockPowerOfTwo);
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
            short chunkIndex;
            int localIndex;
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
                short chunkIndex;
                int localIndex;
                GetChunkedIndex(index, out chunkIndex, out localIndex);
                return _subArrays[chunkIndex][localIndex];
            }
            set
            {
                CheckIndex(index);
                short chunkIndex;
                int localIndex;
                GetChunkedIndex(index, out chunkIndex, out localIndex);
                _subArrays[chunkIndex][localIndex] = value;
            }
        }
    }
}