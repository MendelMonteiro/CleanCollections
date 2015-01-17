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
