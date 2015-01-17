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

namespace CleanCollections.Tests
{
    static class GCTester
    {
        public static void Test(Action test)
        {
            test();

            GC.Collect(GC.MaxGeneration);
            GC.WaitForFullGCComplete();
            var countBefore = GC.CollectionCount(0);

            test();

            var countAfter = GC.CollectionCount(0);
//            Console.WriteLine("Collections before {0} and after {1}", countBefore, countAfter);
            Check.That(countAfter).IsEqualTo(countBefore);
        }
    }
}