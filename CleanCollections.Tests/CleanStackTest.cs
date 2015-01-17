using System;
using System.Collections.Generic;
using NFluent;
using NUnit.Framework;

namespace CleanCollections.Tests
{
    [TestFixture]
    public class CleanStackTest
    {
        [Test]
        public void Test()
        {
            int maxSize = 1024;
            var stack = new CleanStack<int>(maxSize, 128);

            for (int i = 0; i < maxSize; i++)
            {
                stack.Push(i);
                Assert.AreEqual(i + 1, stack.Count);
            }

            for (int i = maxSize - 1; i >= 0; i--)
            {
                var item = stack.Pop();
                Assert.AreEqual(i, item);
            }
        }

        [Test]
        public void TestEnumerate()
        {
            int maxSize = 1024;
            var stack = new CleanStack<int>(maxSize, 128);

            for (int i = 0; i < maxSize; i++)
            {
                stack.Push(i);
                Assert.AreEqual(i + 1, stack.Count);
            }

            int j = 0;
            foreach (var item in stack)
            {
                Assert.AreEqual(j, item);
                j++;
            }
        }

        [Test]
        public void TestClear()
        {
            var stack = new CleanStack<string> (1024, 128);

            stack.Push("blah");
            stack.Clear();

            Check.That(stack).IsEmpty();
            Check.ThatCode(() => stack.Pop()).Throws<InvalidOperationException>();
        }

        [Test]
        public void TestAllocations()
        {
            int maxSize = 1024 * 1024;
            var stack = new CleanStack<int>(maxSize, 128);
            GCTester.Test(() =>
                          {
                              int iterations = 10;
                              for (int j = 0; j < iterations; j++)
                              {
                                  for (int i = 0; i < maxSize; i++)
                                  {
                                      stack.Push(i);
                                  }

                                  for (int i = 0; i < maxSize; i++)
                                  {
                                      stack.Pop();
                                  }
                              }
                          });
        }
    }
}
