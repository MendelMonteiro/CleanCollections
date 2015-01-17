using System.Collections.Generic;
using NFluent;
using NUnit.Framework;

namespace CleanCollections.Tests
{
    [TestFixture]
    class CleanDictionaryTests
    {
        private CleanDictionary<int, string> _dict;
        private CleanDictionary<string, int> _reverseDict;

        [SetUp]
        public void Setup()
        {
            _dict = new CleanDictionary<int, string>(4, maxSize: 1024);
            _reverseDict = new CleanDictionary<string, int>(4, maxSize: 1024);
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
        public void TestAddingStrings()
        {
            for (int i = 1; i < 1023; i++)
            {
                var str = "Hello " + i;
                _reverseDict.Add(str, i);
                Check.That(_reverseDict.Count).IsEqualTo(i);
                Check.That(_reverseDict[str]).IsEqualTo(i);
            }
        }

        [Test]
        public void TestAddingNegativeNumbers()
        {
            for (int i = 1; i < 1023; i++)
            {
                _dict.Add(-i, "Hello " + i);
                Check.That(_dict.Count).IsEqualTo(i);
                Check.That(_dict[-i]).IsEqualTo("Hello " + i);
            }
        }

        [Test]
        public void TestAddingDescendingNumbers()
        {
            int count = 0;
            for (int i = 1023; i > 0; i--)
            {
                _dict.Add(i, "Hello " + i);
                count++;
                Check.That(_dict.Count).IsEqualTo(count);
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
}
