using System;
using System.Collections.Generic;

namespace CleanCollections.Perf
{
    class Program
    {
        static void Main(string[] args)
        {
            int length = 1024 * 1024 * 32;
//            TestSuite.TestList("List", length, new List<int>());
            TestSuite.TestList("Incremental", length, new CleanListIncremental<int>(length, 8192));
//            TestSuite.TestList("Doubling", length, new CleanListDoubling<int>(length, 4));
//            TestSuite.TestList("Exponential", length, new CleanListExponential<int>(length, 4));

            Console.WriteLine("Finished");
            //            Console.ReadKey(true);
        }
    }
}
