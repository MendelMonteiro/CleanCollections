using NFluent;
using NUnit.Framework;

namespace CleanCollections.Tests
{
    static internal class Tester
    {
        public static void TestListEnumerator(IIndexedList<int> list, int length)
        {
            for (int i = 0; i < length; i++)
            {
                list.Add(i);
            }

            int counter = 0;
            foreach (var i in list)
            {
                Assert.AreEqual(i, counter);
                counter++;
            }
        }

        public static void TestListEnumeratorWithOneDelete(IIndexedList<int> list, int deleteAt, int length)
        {
            for (int i = 0; i < length; i++)
            {
                list.Add(i);
            }

            list.RemoveAt(deleteAt);

            int counter = 0;
            foreach (var i in list)
            {
                if (counter == deleteAt)
                {
                    counter++;
                }

                Assert.AreEqual(counter, i);
                counter++;
            }
        }

        public static void TestListEnumeratorWithTwoSpacedDeletes(IIndexedList<int> list, int deleteAt, int length)
        {
            for (int i = 0; i < length; i++)
            {
                list.Add(i);
            }

            list.RemoveAt(deleteAt);
            list.RemoveAt(deleteAt + 10);

            int counter = 0;
            foreach (var i in list)
            {
                if (counter == deleteAt || counter == deleteAt + 10)
                {
                    counter++;
                }

                Assert.AreEqual(counter, i);
                counter++;
            }
        }
    }
}