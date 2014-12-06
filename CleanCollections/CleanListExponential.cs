using System;
using System.Collections;
using System.Collections.Generic;

namespace CleanCollections
{
    /// <summary>
    /// A list that does not produce any garbage when growing and grows exponentially (n ^ initialBlockSize)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CleanListExponential<T> : IIndexedList<T>
    {
        private readonly int _initialBlockSize;
        private readonly CleanQueue<ChunkedIndex> _deletedIndeces;
        private readonly int _maxSize;
        private readonly T[][] _subArrays;
        private int _capacity = 0;
        private int _lastBlockFactor = 1;
        private int _lastChunk = -1;
        private int _count;

        public CleanListExponential(int maxSize, int initialBlockSize)
        {
            _deletedIndeces = new CleanQueue<ChunkedIndex>(maxSize, 2048);
            _maxSize = maxSize;
            _initialBlockSize = initialBlockSize;

            var chunks = (int)Math.Ceiling(Math.Log(_maxSize, _initialBlockSize));
            _subArrays = new T[chunks][];

            EnsureCapacity();
        }

        private static T GetValueAt(int index, T[][] subArrays, int initialBlockSize)
        {
            var chunkIndex = GetChunkIndex(index, subArrays, initialBlockSize);

            var subArray = subArrays[chunkIndex];
            var localIndexOffset = GetLocalIndexOffset(index, chunkIndex, initialBlockSize);

            return subArray[localIndexOffset];
        }

        private static int GetLocalIndexOffset(int index, int chunkIndex, int initialBlockSize)
        {
            var indexStart = chunkIndex == 0 ? 0 : (int) Math.Pow(initialBlockSize, chunkIndex);
            var localIndexOffset = index - indexStart;
            return localIndexOffset;
        }

        private static short GetChunkIndex(int index, T[][] subArrays, int initialBlockSize)
        {
            if (index < initialBlockSize) return 0;
            if (index <= initialBlockSize + 1) return 1;
            var chunkIndex = (short) Math.Floor(Math.Log(index, initialBlockSize));

            if (chunkIndex > subArrays.Length) throw new IndexOutOfRangeException();
            return chunkIndex;
        }

        private static void SetValueAt(int index, T value, T[][] subArrays, int initialBlockSize)
        {
            var chunkIndex = GetChunkIndex(index, subArrays, initialBlockSize);

            var subArray = subArrays[chunkIndex];
            var localIndexOffset = GetLocalIndexOffset(index, chunkIndex, initialBlockSize);

            subArray[localIndexOffset] = value;
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
            int count = _count;
            int absoluteIndex;
            // Use existing entry that was deleted
            if (_deletedIndeces.Count > 0)
            {
                var nextFreeIndex = _deletedIndeces.Dequeue();
                _subArrays[nextFreeIndex.ChunkIndex][nextFreeIndex.LocalIndex] = item;
                absoluteIndex = nextFreeIndex.AbsoluteIndex;
            }
            // Add to the end
            else
            {
                EnsureCapacity();
                SetValueAt(count, item, _subArrays, _initialBlockSize);
                absoluteIndex = count;
            }
            _count = count + 1;
            return absoluteIndex;
        }

        public void Add(T item)
        {
            ((IIndexedList<T>)this).Add(item);
        }

        private void EnsureCapacity()
        {
            if (_count >= _capacity)
            {
                Grow(_lastChunk + 1, _initialBlockSize);
            }
        }

        private void Grow(int nextChunk, int initialBlockSize)
        {
            if (nextChunk > _subArrays.Length)
            {
                throw new NotSupportedException(string.Format("Trying to grow past max size of {0}", _maxSize));
            }

            int factor = nextChunk + 1;
            int lastChunkLength = nextChunk == 0 ? 0 : (int)Math.Pow(initialBlockSize, factor - 1);
            var chunkLength = (int) Math.Pow(initialBlockSize, factor) - lastChunkLength;
            _subArrays[nextChunk] = new T[chunkLength];
            _lastBlockFactor = factor;
            _capacity += chunkLength;
            _lastChunk = nextChunk;
        }

        public void Clear()
        {
            _count = 0;
            // TODO: clear all elements?
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
            /*var chunkIndex = GetChunkIndex()
            _deletedIndeces*/
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
            var absoluteIndex = GetIndex(index);
            _deletedIndeces.Enqueue(absoluteIndex);
            _count--;
            // TODO: set value to default(T) ?
        }

        private ChunkedIndex GetIndex(int index)
        {
            var chunkIndex = GetChunkIndex(index, _subArrays, _initialBlockSize);
            var localIndex = GetLocalIndexOffset(index, chunkIndex, _initialBlockSize);
            return new ChunkedIndex(chunkIndex, localIndex, index);
        }

        public T this[int index]
        {
            get
            {
                CheckIndex(index);
                return GetValueAt(index, _subArrays, _initialBlockSize);
            }
            set
            {
                CheckIndex(index);
                SetValueAt(index, value, _subArrays, _initialBlockSize);
            }
        }

        private void CheckIndex(int index)
        {
            if (index < (uint)0 || (uint)index > (uint)Count - 1) throw new IndexOutOfRangeException();
        }
    }
}