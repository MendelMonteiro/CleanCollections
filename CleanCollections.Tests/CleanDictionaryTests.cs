using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NFluent;
using NUnit.Framework;

namespace CleanCollections.Tests
{
    [TestFixture]
    class CleanDictionaryTests
    {
        [Test]
        public void TestAdding()
        {
            var dict = new CleanDictionary<int,string>(4, maxSize:1024);

            Dictionary<int, string> d;

            dict.Add(1, "Hello");
            Check.That(dict.Count).IsEqualTo(1);
            Check.That(dict[1]).IsEqualTo("Hello");
            //Check.That(dict.Values.First()).IsEqualTo("Hello");
            //Check.That(dict.Keys.First()).IsEqualTo(1);
        }

        [Test]
        public void TestLookup()
        {
            
        }

        [Test]
        public void TestReplace()
        {
            
        }

        [Test]
        public void TestRemove()
        {
            
        }
    }

    /// <summary>
    /// Uses a linked list embedded in the entries list to handle collisions.
    /// Does not allocate once stable.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    internal class CleanDictionary<TKey, TValue> : IDictionary<TKey, TValue>
        where TKey : IEquatable<TKey>
    {
        private readonly IEqualityComparer<TKey> _comparer = EqualityComparer<TKey>.Default;
        private readonly CleanListDoubling<int> _buckets;
        private readonly CleanListDoubling<Entry> _entries;
        private CleanListDoubling<TKey> _keys; // TMP
        private CleanListDoubling<TValue> _values; // TMP
        private int _size;
        private int _nextFreeSlot = -1;
        private int _capacity;
        private bool _isInitialised;

        public CleanDictionary(int initialCapcity, int blockSize = 16, int maxSize = int.MaxValue)
        {
            _capacity = initialCapcity;
            _buckets = new CleanListDoubling<int>(maxSize, blockSize);
            _entries = new CleanListDoubling<Entry>(maxSize, blockSize);
            Initialise();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            InsertItem(item.Key, item.Value);
        }

        /// <summary>
        /// Setup the buck
        /// </summary>
        private void Initialise()
        {
            if (_isInitialised)
                return;

            for (int i = 0; i < _capacity; i++)
            {
                _buckets.Add(-1);
            }
        }

        /// <summary>
        /// Find the bucket
        /// If the bucket is empty add the item to the slot
        /// Otherwise find the next free slot and add the item there whilst pointing the current slot to this slot 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private void InsertItem(TKey key, TValue value)
        {
            // Fuck it if we're going to raise an exception it may as well be a null-ref (avoid boxing)
            var hashcode = key.GetHashCode(); // Could use comparer but we want the exception
            var bucket = GetBucket(hashcode);

            // New slot in new bucket
            if (_buckets[bucket] < 0)
            {
                _buckets[bucket] = AddNewSlot(key, value, hashcode);
                return;
            }

            Entry slot;
            for (slot = _entries[bucket]; !_comparer.Equals(key, slot.Key); slot = _entries[slot.Next])
            {
                // New slot in existing bucket
                if (slot.Next == -1)
                {
                    slot.Next = AddNewSlot(key, value, hashcode);
                    return;
                }
            }

            // Replacing slot with new value for same key
            slot.Value = value;
            slot.Key = key; // Need to overwrite the key?
        }

        private int AddNewSlot(TKey key, TValue value, int hashcode)
        {
            var newSlot = new Entry();
            newSlot.Next = -1;
            newSlot.Key = key;
            newSlot.Value = value;
            newSlot.HashCode = hashcode;

            EnsureCapacity();
            return AddEntry(newSlot);
        }

        // TODO: refactor this to return the next entry to use instead
        private int AddEntry(Entry newEntry)
        {
            _size++;

            if (_nextFreeSlot >= 0)
            {
                var freeEntry = _entries[_nextFreeSlot];
                var addedSlot = _nextFreeSlot;
                _entries[addedSlot] = newEntry;
                _nextFreeSlot = freeEntry.Next >= 0 ? freeEntry.Next : -1;
                return addedSlot;
            }

            return ((IIndexedList<Entry>) _entries).Add(newEntry);
        }

        /// <summary>
        /// To be called before adding items to _entries
        /// </summary>
        private void EnsureCapacity()
        {
            if (_size >= _entries.Count)
            {
                Grow();
            }
        }

        private int GetBucket(int hashcode)
        {
            return _capacity % hashcode;
        }

        /// <summary>
        /// Double the capacity and then re-hash using the new capacity
        /// Start from the last bucket and work our way down to do this in-place
        /// </summary>
        private void Grow()
        {
            var oldCapacity = _capacity;
            _capacity = oldCapacity*2;
            for (int i = oldCapacity; i < _capacity; i++)
            {
                _buckets.Add(-1);
            }

            for (int i = oldCapacity - 1; i >= 0; i--)
            {
                var slotIndex = _buckets[i];
                if (slotIndex > 0)
                {
                    var slot = _entries[slotIndex];
                    var newBucket = GetBucket(slot.HashCode);
                    _buckets[newBucket] = slotIndex;
                    _buckets[i] = -1;
                }
            }
        }

        /// <summary>
        /// Set the used slot to be the next free slot and point the current free slot to the used slot.
        /// Find the slot and de-reference the entry 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool RemoveItem(TKey key, out TValue value)
        {
            value = default(TValue);
            return false;
        }

        /// <summary>
        /// Zero out all arrays and reset size to 0
        /// </summary>
        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            TValue value;
            return TryGetValue(item.Key, out value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public int Count { get { return _size; } }

        public bool IsReadOnly { get { return false; } }

        /// <summary>
        /// Find bucket, check if slot exists
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            throw new NotImplementedException();
        }

        public void Add(TKey key, TValue value)
        {
            InsertItem(key, value);
        }

        public bool Remove(TKey key)
        {
            TValue value;
            return RemoveItem(key, out value);
        }

        /// <summary>
        /// Find the bucket, check if a slot exists
        /// If it does, follow the linked list until util we find a key that equals the other key
        /// Return the entry value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            throw new NotImplementedException();
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue value;
                if (TryGetValue(key, out value))
                    return value;

                throw new KeyNotFoundException();
            }
            set
            {
                InsertItem(key, value);
            }
        }

        /// <summary>
        /// Walk all the entries and return the keys
        /// </summary>
        public ICollection<TKey> Keys { get { return _keys; } }

        /// <summary>
        /// Walk all the entries and return the values
        /// </summary>
        public ICollection<TValue> Values { get { return _values; } }

        private struct Entry
        {
            public int HashCode;
            public int Next;
            public TKey Key;
            public TValue Value;
        }
    }
}
