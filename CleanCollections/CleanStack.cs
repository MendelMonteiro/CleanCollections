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

using System;
using System.Collections;
using System.Collections.Generic;

namespace CleanCollections
{
    /// <summary>
    /// A stack that grows linearly without creating any garbage.
    /// Similar ot CleanListIncremental but exposes a Stack interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CleanStack<T> : IEnumerable<T>
    {
        private readonly int _blockSize;
        private readonly T[][] _subArrays;
        private int _count;
        private int _capacity;
        private readonly int _blockPowerOfTwo;
        private short _headChunkIndex;
        private int _headLocalIndex = -1;

        public CleanStack(int maxSize, int blockSize)
        {
            if (!Util.IsPowerOfTwo(blockSize)) throw new ArgumentException("blockSize must be a power of two");

            _blockPowerOfTwo = (int)Math.Log(blockSize, 2);

            _blockSize = blockSize;

            _subArrays = new T[(short)(maxSize / blockSize)][];
        }

        public void Push(T item)
        {
            EnsureCapacity();

            _headChunkIndex = GetNextChunkAndLocalIndex(_headChunkIndex, _headLocalIndex, out _headLocalIndex);

            _subArrays[_headChunkIndex][_headLocalIndex] = item;
            _count++;
        }

        private short GetNextChunkAndLocalIndex(short headChunkIndex, int headLocalIndex, out int localIndex)
        {
            localIndex = ++headLocalIndex;
            
            if (headLocalIndex >= _subArrays[headChunkIndex].Length)
            {
                localIndex = _headLocalIndex = 0;
                return ++headChunkIndex;
            }
            
            return headChunkIndex;
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

        public T Pop()
        {
            if (_count <= 0) throw new InvalidOperationException("Stack is empty");

            short chunkIndex;
            int localIndex;
            var item = GetCurrentItem(out chunkIndex, out localIndex);

            // Set current item to default(T)
            _subArrays[chunkIndex][localIndex] = default(T);

            _count--;

            _headLocalIndex--;
            if (_headLocalIndex < 0 && _headChunkIndex > 0)
            {
                _headChunkIndex--;
                _headLocalIndex = _subArrays[_headChunkIndex].Length - 1;
            }

            return item;
        }

        private T GetCurrentItem(out short chunkIndex, out int localIndex)
        {
            chunkIndex = _headChunkIndex;
            localIndex = _headLocalIndex;
            var item = _subArrays[chunkIndex][localIndex];
            return item;
        }

        private T GetItem(int index, out short chunkIndex, out int localIndex)
        {
            chunkIndex = (short)(index >> _blockPowerOfTwo);
            localIndex = index - (chunkIndex * _blockSize);
            var item = _subArrays[chunkIndex][localIndex];
            return item;
        }

        public void Clear()
        {
            _count = 0;
            _headChunkIndex = 0;
            _headLocalIndex = -1;

            foreach (var subArray in _subArrays)
            {
                if (subArray != null) Array.Clear(subArray, 0, subArray.Length);
            }
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
            private readonly CleanStack<T> _stack;
            private T _current;
            private int _index;
            private short _chunkIndex;
            private int _localIndex;

            public Enumerator(CleanStack<T> stack)
            {
                _index = 0;
                _stack = stack;
                _current = default(T);
                _chunkIndex = 0;
                _localIndex = -1;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                bool beforeEnd = _index < _stack.Count;

                if (beforeEnd)
                {
                    _index++;
                    _chunkIndex = _stack.GetNextChunkAndLocalIndex(_chunkIndex, _localIndex, out _localIndex);
                    _current = _stack._subArrays[_chunkIndex][_localIndex];
                }

                return beforeEnd;
            }

            public void Reset()
            {
                _index = 0;
                _chunkIndex = 0;
                _localIndex = -1;
            }

            public T Current { get { return _current; } private set { _current = value; } }

            object IEnumerator.Current { get { return Current; } }
        }
    }
}
