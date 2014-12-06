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
            var dict = new CleanDictionary<int,string>();

            dict.Add(1, "Hello");
            Check.That(dict).HasSize(1);
            Check.That(dict.Values.First()).IsEqualTo("Hello");
            Check.That(dict.Keys.First()).IsEqualTo(1);
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

    internal class CleanDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
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
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public int Count { get; private set; }
        public bool IsReadOnly { get; private set; }
        public bool ContainsKey(TKey key)
        {
            throw new NotImplementedException();
        }

        public void Add(TKey key, TValue value)
        {
            throw new NotImplementedException();
        }

        public bool Remove(TKey key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            throw new NotImplementedException();
        }

        public TValue this[TKey key] { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public ICollection<TKey> Keys { get; private set; }
        public ICollection<TValue> Values { get; private set; }
    }
}
