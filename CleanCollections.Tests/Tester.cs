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

using NUnit.Framework;

namespace CleanCollections.Tests
{
    static internal class Tester
    {
        public static void TestListEnumerator(this IIndexedList<int> list, int length)
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

        public static void TestListEnumeratorWithOneDelete(this IIndexedList<int> list, int deleteAt, int length)
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

        public static void TestListEnumeratorWithTwoSpacedDeletes(this IIndexedList<int> list, int deleteAt, int length)
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