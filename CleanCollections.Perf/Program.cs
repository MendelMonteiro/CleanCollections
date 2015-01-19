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
using System.Collections.Generic;
using System.Diagnostics;

namespace CleanCollections.Perf
{
    class Program
    {
        static void Main(string[] args)
        {
            int length = 1024 * 1024 * 32;
//            TestSuite.TestList("List", length, new List<int>());
//            TestSuite.TestList("Incremental", length, new CleanListIncremental<int>(length, 8192));
//            TestSuite.TestList("Doubling", length, new CleanListDoubling<int>(length, 4));
//            TestSuite.TestList("Exponential", length, new CleanListExponential<int>(length, 4));

//            CleanStackTest test = new CleanStackTest();
//            test.TestAllocations();

//            CleanListIncrementalTest test = new CleanListIncrementalTest(); 
//            test.TestAllocations();
            
//            TestDictionary();
//            TestDictionary();

            TestStack();

            Console.WriteLine("Finished");
            //            Console.ReadKey(true);
        }

        public static void TestStack()
        {
            Stack<int> stack = new Stack<int>();
            var iterations = 5 * 1024 * 1024;
            CleanStack<int> cleanStack = new CleanStack<int>(iterations, 8192);

            Stopwatch watch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                cleanStack.Push(i);
            }
            TestSuite.PrintTimeTaken("Push", iterations, watch);

            watch.Restart();
            foreach (var i in cleanStack)
            {
                var x = i;
            }
            TestSuite.PrintTimeTaken("Iterate", iterations, watch);

            watch.Restart();
            for (int i = 0; i < iterations; i++)
            {
                cleanStack.Pop();
            }
            TestSuite.PrintTimeTaken("Push", iterations, watch);
        }


        private static void TestDictionary()
        {
            var iterations = 5*1024*1024;
            var dict = new CleanDictionary<int, int>(4, iterations, 512);
            for (int i = 0; i < iterations; i++)
            {
                dict.Add(i, i);
            }

//            Console.ReadKey();

            for (int i = 0; i < iterations; i++)
            {
                var x = dict[i];
            }
        }
    }
}
