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
using System.Collections.Generic;
using System.Diagnostics;
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
            for (int i = 1; i < 1024; i++)
            {
                _dict.Add(i, "Hello " + i);
                Check.That(_dict.Count).IsEqualTo(i);
                Check.That(_dict[i]).IsEqualTo("Hello " + i);
            }
        }

        [Test]
        public void TestAddingStrings()
        {
            for (int i = 1; i < 1024; i++)
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
            for (int i = 1; i < 1024; i++)
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
            for (int i = 1024; i > 0; i--)
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
            for (int i = 0; i < 17; i++)
            {
                _dict.Add(i, i.ToString());
            }

            Check.That(_dict.Count).IsEqualTo(17);

            for (int i = 0; i < _dict.Count; i++)
            {
                Check.That(_dict[i]).IsEqualTo(i.ToString());
            }
        }

        [Test]
        public void TestLookup()
        {
            for (int i = 0; i < 1024; i++)
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

            for (int i = 1; i < 1024; i++)
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

        [Test, Explicit]
        public void TestPerformanceVsDictionary()
        {
            const int maxSize = 3 * 1024 * 1024;
            //            RunPerfTest(maxSize);
            //        }

            //        private static void RunPerfTest(int maxSize)
            //        {
            var clean = new CleanDictionary<int, int>(1024, maxSize);
            var dirty = new Dictionary<int, int>(1024);

            RunPerfOperation(maxSize, dirty, clean, (ints, i) => ints.Add(i, i), (ints, i) => ints.Add(i, i), "add/grow");

            RunPerfOperation(maxSize, dirty, clean, (ints, i) => ints.Remove(i), (ints, i) => ints.Remove(i), "remove");

            RunPerfOperation(maxSize, dirty, clean, (ints, i) => ints.Add(i, i), (ints, i) => ints.Add(i, i), "add");

            RunPerfOperation(maxSize, dirty, clean,
                             (ints, i) =>
                             {
                                 var x = ints[i];
                             },
                             (ints, i) =>
                             {
                                 var x = ints[i];
                             }, "index");

            RunPerfOperation(maxSize, dirty, clean, (ints, i) => ints[i] = 3, (ints, i) => ints[i] = 3, "set");
        }

        private static void RunPerfOperation(int maxSize, Dictionary<int, int> dirty, CleanDictionary<int, int> clean, Action<Dictionary<int, int>, int> dirtyAction, Action<CleanDictionary<int, int>, int> cleanAction, string operationName)
        {
            Stopwatch watch = Stopwatch.StartNew();
            for (int i = 0; i < maxSize; i++)
            {
                dirtyAction(dirty, i);
            }
            var dirtyTicks = watch.ElapsedTicks;
            Console.WriteLine(dirtyTicks);

            watch = Stopwatch.StartNew();
            for (int i = 0; i < maxSize; i++)
            {
                cleanAction(clean, i);
            }
            var cleanTicks = watch.ElapsedTicks;
            Console.WriteLine(cleanTicks);

            Console.WriteLine("Clean dictionary took {0:P} more time to {1} than dirty one", (double)(cleanTicks - dirtyTicks) / dirtyTicks, operationName);
        }

        [Test]
        public void TestAllocations()
        {
            const int maxSize = 1024 * 1024;
            var dict = new CleanDictionary<int, int>(16, maxSize: maxSize);
            GCTester.Test(() =>
                          {
                              for (int i = 0; i < maxSize; i++)
                              {
                                  dict.Add(i, i);
                              }

                              for (int i = 0; i < maxSize; i++)
                              {
                                  var x = dict[i];
                                  dict.Remove(i);
                              }
                          });
        }
    }
}
