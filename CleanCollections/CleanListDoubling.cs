using System;
using System.Collections;
using System.Collections.Generic;

namespace CleanCollections
{
    /// <summary>
    /// A list that does not produce any garbage when growing and grows by a doubling the next block size.
    /// 
    /// c => capacity
    /// s => startIndex
    /// b => block size
    /// i => block index
    /// 
    /// c = [b * (2 ^ i-1)] - b - 1
    /// s = [b * (2 ^ i)] - b
    /// 
    /// c + b = b * (2 ^ i)
    /// (c + b) / b = 2 ^ i
    /// i = log2((c + b) / b)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CleanListDoubling<T> : IIndexedList<T>
    {
        private readonly int _blockSize;
        private readonly CleanStack<ChunkedIndex> _deletedIndeces;
        private int _count;
        private readonly T[][] _subArrays;
        private int _capacity;
        private int _lastChunk = -1;

        public CleanListDoubling(int maxSize, int blockSize)
        {
            _deletedIndeces = new CleanStack<ChunkedIndex>(maxSize, 2048);
            _blockSize = blockSize;

            var blocks = (int)Math.Log((maxSize + blockSize + 1)/(double)blockSize, 2) + 1;
            _subArrays = new T[blocks][];
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
            int absoluteIndex;

            // Use existing entry that was deleted
            if (_deletedIndeces.Count > 0)
            {
                var nextFreeIndex = _deletedIndeces.Pop();
                _subArrays[nextFreeIndex.ChunkIndex][nextFreeIndex.LocalIndex] = item;
                absoluteIndex = nextFreeIndex.AbsoluteIndex;
            }
            else
            {
                EnsureCapacity();

                short chunkIndex;
                int localIndex;
                GetChunkedIndex(count, out chunkIndex, out localIndex);

                _subArrays[chunkIndex][localIndex] = item;
                absoluteIndex = count;
            }

            _count = count + 1;
            return absoluteIndex;
        }

        private void GetChunkedIndex(int index, out short chunkIndex, out int localIndex)
        {
            chunkIndex = (short) Util.LogDeBruijn((int) ((index + _blockSize) / (double)_blockSize));
            var startIndex = (_blockSize << chunkIndex) - _blockSize;
            localIndex = index - startIndex;
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
            int currentBlockSize = _lastChunk == 0 ? blockSize : blockSize << _lastChunk;
            _subArrays[_lastChunk] = new T[currentBlockSize];
            _capacity += currentBlockSize;
        }

        public void Add(T item)
        {
            ((IIndexedList<T>)this).Add(item);
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
            _deletedIndeces.Push(new ChunkedIndex(chunkIndex, localIndex, index));
            _count--;
            // TODO: set value to default(T) ?
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

        private void CheckIndex(int index)
        {
            if (index < (uint)0 || (uint)index > (uint)Count - 1) throw new IndexOutOfRangeException();
        }

    }
}
