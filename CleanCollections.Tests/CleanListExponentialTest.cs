using NUnit.Framework;

namespace CleanCollections.Tests
{
    [TestFixture]
    public class CleanListExponentialTest
    {
        [Test]
        public void TestEnumerator()
        {
            int length = 1024;
            var list = new CleanListExponential<int>(length, 4);

            Tester.TestListEnumerator(list, length);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(500)]
        [TestCase(1022)]
        [TestCase(1023)]
        public void TestEnumeratorWithOneDelete(int deleteAt)
        {
            int length = 1024;
            var list = new CleanListExponential<int>(length, 4);

            Tester.TestListEnumeratorWithOneDelete(list, deleteAt, length);
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
            var list = new CleanListExponential<int>(length, 4);
            Tester.TestListEnumeratorWithTwoSpacedDeletes(list, deleteAt, length);
        }
    }
}
