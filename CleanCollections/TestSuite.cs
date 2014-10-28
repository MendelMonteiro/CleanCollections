using System;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;

namespace CleanCollections
{
    [TestFixture]
    public class TestSuite
    {
        [Test]
        public void Main()
        {
            int length = 1024 * 1024 * 4;
            TestList("List", length, new List<int>());
            TestList("Incremental", length, new CleanListIncremental<int>(length, 8192));
            TestList("Doubling", length, new CleanListDoubling<int>(length, 4));
            TestList("Exponential", length, new CleanListExponential<int>(length, 4));

            Console.WriteLine("Finished");
        }

        public static void TestList(string type, int length, IList<int> list)
        {
            Stopwatch watch = Stopwatch.StartNew();
            // Add
            for (int i = 0; i < length; i++)
            {
                list.Add(i);
            }

            // Indexer
            for (int i = 0; i < length; i++)
            {
                int valueAt = list[i];
                //                Assert.AreEqual(i, valueAt);
            }

            // Enumerate
            foreach (var i in list)
            {
                int blah = i + 1;
            }

            // Remove
            int count = list.Count;
            for (int i = count -1; i >= 0; i--)
            {
                list.RemoveAt(i);
            }

            Assert.AreEqual(0, list.Count, "List is not empty");

            // Add again to test delete queue
            for (int i = 0; i < length; i++)
            {
                list.Add(i);
            }
            watch.Stop();

            var nsPerItem = (double) watch.ElapsedTicks*1000*1000*1000/Stopwatch.Frequency/length;
            Console.WriteLine("{2} took {0:N} ms ({1:N} ns per item)", watch.ElapsedMilliseconds, nsPerItem, type);
        }
    }
}
