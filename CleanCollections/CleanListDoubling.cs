using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCollections
{
    /// <summary>
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
        private readonly int _maxSize;
        private readonly Queue<ChunkedIndex> _deletedIndeces = new Queue<ChunkedIndex>();
        private int _count;
        private readonly T[][] _subArrays;
        private int _capacity;
        private int _lastChunk = -1;

        public CleanListDoubling(int maxSize, int blockSize)
        {
            _blockSize = blockSize;
            _maxSize = maxSize;

            var blocks = (int)Math.Log((maxSize + blockSize + 1)/(double)blockSize, 2) + 1;
            _subArrays = new T[blocks][];
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new CleanListEnumerator<T>(this, _deletedIndeces);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static readonly int[] MultiplyDeBruijnBitPosition = new int[32]
                                                                    {
                                                                        0, 9, 1, 10, 13, 21, 2, 29, 11, 14, 16, 18, 22, 25, 3, 30,
                                                                        8, 12, 20, 28, 15, 17, 24, 7, 19, 27, 23, 6, 26, 5, 4, 31
                                                                    };

        private static int LogDeBruijn(int v)
        {
            v |= v >> 1; // first round down to one less than a power of 2 
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;

            return MultiplyDeBruijnBitPosition[(uint) (v*0x07C4ACDDU) >> 27];
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

        private void GetChunkedIndex(int index, out int chunkIndex, out int localIndex)
        {
            chunkIndex = LogDeBruijn((int) ((index + _blockSize) / (double)_blockSize));
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
            int chunkIndex;
            int localIndex;
            GetChunkedIndex(index, out chunkIndex, out localIndex);
            _deletedIndeces.Enqueue(new ChunkedIndex(chunkIndex, localIndex, index));
            _count--;
            // TODO: set value to default(T) ?
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

        private void CheckIndex(int index)
        {
            if (index < (uint)0 || (uint)index > (uint)Count - 1) throw new IndexOutOfRangeException();
        }

    }
}
