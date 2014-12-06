using NUnit.Framework;

namespace CleanCollections.Tests
{
    [TestFixture]
    public class CleanListIncrementalTest
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
            var list = new CleanListIncremental<int>(length, 4);

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
            var list = new CleanListIncremental<int>(length, 4);
            list.TestListEnumeratorWithTwoSpacedDeletes(deleteAt, length);
        }

        [Test]
        public void TestAllocations()
        {
            const int maxSize = 1024*1024;
            var list = new CleanListIncremental<int>(maxSize, 128);
            GCTester.Test(() => list.AddThenRemove(maxSize));
        }

        [Test]
        public void TestEnumerator()
        {
            int length = 1024;
            var list = new CleanListIncremental<int>(length, 4);

            list.TestListEnumerator(length);
        }
    }
}