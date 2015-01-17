using System;
using System.Collections.Generic;
using CleanCollections.Tests;

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
            
            var iterations = 5 * 1024 * 1024;
            var dict = new CleanDictionary<int, int>(4, iterations, 512);
            for (int i = 0; i < iterations; i++)
            {
                dict.Add(i, i);
            }

            Console.ReadKey();

            for (int i = 0; i < iterations; i++)
            {
                var x = dict[i];
            }

            Console.WriteLine("Finished");
            //            Console.ReadKey(true);
        }
    }
}
