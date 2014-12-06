using NUnit.Framework;

namespace CleanCollections.Tests
{
    [TestFixture]
    public class CleanQueueTest
    {
        [Test]
        public void Test()
        {
            int maxSize = 1024;
            var queue = new CleanQueue<int>(maxSize, 128);

            for (int i = 0; i < maxSize; i++)
            {
                queue.Enqueue(i);
                Assert.AreEqual(i + 1, queue.Count);
            }

            for (int i = maxSize - 1; i >= 0; i--)
            {
                var item = queue.Dequeue();
                Assert.AreEqual(i, item);
            }
        }

        [Test]
        public void TestEnumerate()
        {
            int maxSize = 1024;
            var queue = new CleanQueue<int>(maxSize, 128);

            for (int i = 0; i < maxSize; i++)
            {
                queue.Enqueue(i);
                Assert.AreEqual(i + 1, queue.Count);
            }

            int j = 0;
            foreach (var item in queue)
            {
                Assert.AreEqual(j, item);
                j++;
            }
        }

        [Test]
        public void TestAllocations()
        {
            int maxSize = 1024 * 1024;
            var queue = new CleanQueue<int>(maxSize, 128);
            GCTester.Test(() =>
                          {
                              int iterations = 1000;
                              for (int j = 0; j < iterations; j++)
                              {
                                  for (int i = 0; i < maxSize; i++)
                                  {
                                      queue.Enqueue(i);
                                  }

                                  for (int i = 0; i < maxSize; i++)
                                  {
                                      queue.Dequeue();
                                  }
                              }
                          });
        }
    }
}
