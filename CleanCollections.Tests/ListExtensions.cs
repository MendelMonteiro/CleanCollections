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

using System.Collections.Generic;

namespace CleanCollections.Tests
{
    static internal class ListExtensions
    {
        public static void AddThenRemove(this IList<int> list, int maxSize)
        {
            const int iterations = 10;
            for (var j = 0; j < iterations; j++)
            {
                for (var i = 0; i < maxSize; i++)
                {
                    list.Add(i);
                }

                for (var i = maxSize - 1; i >= 0; i--)
                {
                    list.RemoveAt(i);
                }
            }
        }
    }
}