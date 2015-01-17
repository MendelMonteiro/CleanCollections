using System;
using NFluent;
using NUnit.Framework;

namespace CleanCollections.Tests
{
    [TestFixture]
    public class CleanListDoublingTest
    {
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(500)]
        [TestCase(1022)]
        [TestCase(1023)]
        public void TestEnumeratorWithOneDelete(int deleteAt)
        {
            int length = 1024;
            var list = new CleanListDoubling<int>(length, 4);

            list.TestListEnumeratorWithOneDelete(deleteAt, length);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(500)]
        [TestCase(1012)]
        [TestCase(1013)]
        public void TestEnumeratorWithTwoSpacedDeletes(int deleteAt)
        {
            int length = 1024;
            var list = new CleanListDoubling<int>(length, 4);
            list.TestListEnumeratorWithTwoSpacedDeletes(deleteAt, length);
        }

        [Test]
        public void TestAllocations()
        {
            const int maxSize = 1024*1024;
            var list = new CleanListDoubling<int>(maxSize, 128);
            GCTester.Test(() => list.AddThenRemove(maxSize));
        }

        [Test]
        public void TestEnumerator()
        {
            const int length = 1024;
            var list = new CleanListDoubling<int>(length, 4);

            list.TestListEnumerator(length);
        }

        [Test]
        public void TestClear()
        {
            var list = new CleanListDoubling<int>(1024, 4);            
            list.Add(1);
            list.Clear();

            Check.That(list.Count).IsEqualTo(0);
            Check.ThatCode(() => list[0]).Throws<IndexOutOfRangeException>();
        }

        [Test]
        public void TestSetRange()
        {
            var cleanList = new CleanListDoubling<int>(1024, 16);
            IIndexedList<int> list = cleanList;
            for (int i = 0; i < 1024; i++)
            {
                list[list.Add(i)] = 2;
            }

            cleanList.SetRange(0, 1024, 5);

            for (int i = 0; i < cleanList.Count; i++)
            {
                Check.That(cleanList[i]).IsEqualTo(5);
            }
        }
    }
}