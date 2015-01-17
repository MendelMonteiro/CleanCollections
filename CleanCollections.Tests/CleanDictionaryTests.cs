using System;
using System.Collections;
using System.Collections.Generic;
using NFluent;
using NUnit.Framework;

namespace CleanCollections.Tests
{
    [TestFixture]
    class CleanDictionaryTests
    {
        private CleanDictionary<int, string> _dict;

        [SetUp]
        public void Setup()
        {
            _dict = new CleanDictionary<int, string>(4, maxSize: 1024);
        }

        [Test]
        public void TestAdding()
        {
            for (int i = 1; i < 1023; i++)
            {
                _dict.Add(i, "Hello " + i);
                Check.That(_dict.Count).IsEqualTo(i);
                Check.That(_dict[i]).IsEqualTo("Hello " + i);
            }
        }

        [Test]
        public void TestShouldGrow()
        {
            for (int i = 0; i < 5; i++)
            {
                _dict.Add(i, i.ToString());
            }

            Check.That(_dict.Count).IsEqualTo(5);

            for (int i = 0; i < _dict.Count; i++)
            {
                Check.That(_dict[i]).IsEqualTo(i.ToString());
            }
        }

        [Test]
        public void TestLookup()
        {
            for (int i = 0; i < 1023; i++)
            {
                var key = 2 + i;
                _dict.Add(key, "Hello " + key);
                Check.That(_dict[key]).IsEqualTo("Hello " + key);
            }
        }

        [Test]
        public void TestLookupMissingKey()
        {
            _dict.Add(1, "Foo");

            Check.ThatCode(() => _dict[2]).Throws<KeyNotFoundException>();
        }

        [Test]
        public void TestEmptyDictionary()
        {
            Check.That(_dict.Count).IsEqualTo(0);
            Check.ThatCode(() => _dict[3]).Throws<KeyNotFoundException>();
            Check.ThatCode(() => _dict.Remove(3)).Throws<KeyNotFoundException>();
            Check.That(_dict.ContainsKey(3)).IsFalse();
        }

        [Test]
        public void TestReplace()
        {
            _dict.Add(1, "Hello");

            _dict[1] = "Goodbye";

            Check.That(_dict.Count).IsEqualTo(1);
            Check.That(_dict[1]).IsEqualTo("Goodbye");
        }

        [Test]
        public void TestRemove()
        {
            TestAdding();

            for (int i = 1; i < 1023; i++)
            {
                _dict.Remove(i);
            }

            TestEmptyDictionary();
        }

        [Test]
        public void TestRemoveThenAdd()
        {
            TestRemove();
            TestAdding();
        }

        [Test]
        public void TestClear()
        {
            TestAdding();
            _dict.Clear();
            TestEmptyDictionary();
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
            var hashcode = GetKeyHashcode(key);
            var bucket = GetBucket(hashcode);

            // New slot in new bucket
            if (_buckets[bucket] < 0)
            {
                _buckets[bucket] = AddNewSlot(key, value, hashcode, ref bucket);
                return;
            }

            Entry slot;
            for (slot = _entries[bucket]; !_comparer.Equals(key, slot.Key); slot = _entries[slot.Next])
            {
                // New slot in existing bucket
                if (slot.Next == -1)
                {
                    NewSlotInExistingBucket(key, value, bucket, hashcode, slot);
                    return;
                }
            }

            // Replacing slot with new value for same key
            slot.Value = value;
            slot.Key = key; // Need to overwrite the key?
            _entries[bucket] = slot;
        }

        private void NewSlotInExistingBucket(TKey key, TValue value, int bucket, int hashcode, Entry slot)
        {
            var previousBucket = bucket;
            slot.Next = AddNewSlot(key, value, hashcode, ref bucket);
            if (bucket == previousBucket)
                _entries[previousBucket] = slot;
            else
                _buckets[bucket] = slot.Next;
        }

        private int AddNewSlot(TKey key, TValue value, int hashcode, ref int bucket)
        {
            var newSlot = new Entry();
            newSlot.Next = -1;
            newSlot.Key = key;
            newSlot.Value = value;
            newSlot.HashCode = hashcode;

            EnsureCapacity(hashcode, ref bucket);
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

            return ((IIndexedList<Entry>)_entries).Add(newEntry);
        }

        /// <summary>
        /// To be called before adding items to _entries
        /// </summary>
        /// <param name="hashcode"></param>
        /// <param name="bucket"></param>
        private void EnsureCapacity(int hashcode, ref int bucket)
        {
            if (_capacity <= _entries.Count)
            {
                Grow();

                // Re-hash the current if we grow
                bucket = GetBucket(hashcode);
            }
        }

        private int GetBucket(int hashcode)
        {
            return hashcode % _capacity;
        }

        /// <summary>
        /// Double the capacity and then re-hash using the new capacity
        /// Start from the last bucket and work our way down to do this in-place
        /// When an entry moves into a vacant bucket make sure to change any entries that point to it
        /// </summary>
        private void Grow()
        {
            var oldCapacity = _capacity;
            _capacity = oldCapacity * 2;
            for (int i = oldCapacity; i < _capacity; i++)
            {
                _buckets.Add(-1);
            }

            for (int bucketIndex = oldCapacity - 1; bucketIndex >= 0; bucketIndex--)
            {
                var slotIndex = _buckets[bucketIndex];
                if (slotIndex <= 0) continue;

                int previous = -1;
                for (int current = slotIndex; current > 0; )
                {
                    var currentSlot = _entries[current];
                    var next = currentSlot.Next;

                    var newBucketIndex = GetBucket(currentSlot.HashCode);
                    if (newBucketIndex != bucketIndex)
                        MoveSlotFromBucketToNewBucket(newBucketIndex, current, previous, currentSlot, bucketIndex);

                    previous = current;
                    current = next;
                }
            }
        }

        private void MoveSlotFromBucketToNewBucket(int newBucket, int current, int previous, Entry currentSlot, int i)
        {
            _buckets[newBucket] = current;
            // Moving a slot referenced by a previous one
            if (previous > 0)
            {
                var previousSlot = _entries[previous];
                previousSlot.Next = currentSlot.Next;
                _entries[previous] = previousSlot;
            }
            // Moving a slot not referenced
            else
            {
                _buckets[i] = currentSlot.Next;
                currentSlot.Next = -1;
                _entries[current] = currentSlot;
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
            if (_size <= 0) ThrowKeyNotFound(key);

            value = default(TValue);
            var hashcode = GetKeyHashcode(key);
            var bucket = GetBucket(hashcode);

            var slotIndex = _buckets[bucket];
            if (slotIndex < 0)
                return false;

            Entry slot;
            int previousSlotIndex = -1, currentSlotIndex = slotIndex;
            for (slot = _entries[slotIndex]; !_comparer.Equals(key, slot.Key); slot = _entries[slot.Next], currentSlotIndex = slot.Next)
            {
                if (slot.Next < 0)
                    return false; // Got to end of linked list without finding matching entry
                
                previousSlotIndex = slotIndex;
            }

            // Found the entry
            value = slot.Value;

            RemoveSlot(previousSlotIndex, slot, bucket, currentSlotIndex);

            return true;
        }

        private void RemoveSlot(int previousSlotIndex, Entry slot, int bucket, int currentSlotIndex)
        {
            // The slot will either be on its own, at the start, at the end or in the middle
            if (previousSlotIndex < 0 && slot.Next < 0)
            {
                _buckets[bucket] = -1;
            }
            else if (previousSlotIndex < 0 && slot.Next >= 0)
            {
                _buckets[bucket] = slot.Next;
            }
            else if (slot.Next < 0 && previousSlotIndex >= 0)
            {
                var previous = _entries[previousSlotIndex];
                previous.Next = -1;
                _entries[previousSlotIndex] = previous;
            }
            else
            {
                var previous = _entries[previousSlotIndex];
                previous.Next = slot.Next;
                _entries[previousSlotIndex] = previous;
            }

            _entries[currentSlotIndex] = new Entry{Next = _nextFreeSlot};
            _nextFreeSlot = currentSlotIndex;
            _size--;
        }

        /// <summary>
        /// Zero out all arrays and reset size to 0
        /// </summary>
        public void Clear()
        {
            _buckets.Clear();
            _entries.Clear();
            _size = 0;
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
            TValue value;
            return TryGetValue(key, out value);
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
            value = default(TValue);

            if (_size <= 0) return false;

            var hashcode = GetKeyHashcode(key);
            var bucket = GetBucket(hashcode);

            var slotIndex = _buckets[bucket];
            if (slotIndex < 0)
                return false;

            Entry slot;
            for (slot = _entries[slotIndex]; !_comparer.Equals(key, slot.Key); slot = _entries[slot.Next])
            {
                if (slot.Next < 0) return false;
            }

            value = slot.Value;
            return true;
        }

        private void ThrowKeyNotFound(TKey key)
        {
            throw new KeyNotFoundException(string.Format("Key {0} not found", key));
        }

        private static int GetKeyHashcode(TKey key)
        {
            // Fuck it if we're going to raise an exception it may as well be a null-ref (avoid boxing)
            var hashcode = key.GetHashCode(); // Could use comparer but we want the exception
            return hashcode;
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

            public override string ToString()
            {
                return string.Format("Key: {0}, Value: {1}, Next: {2}, HashCode: {3}", Key, Value, Next, HashCode);
            }
        }
    }
}
