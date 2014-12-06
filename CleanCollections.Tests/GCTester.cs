using System;
using NFluent;

namespace CleanCollections.Tests
{
    static class GCTester
    {
        public static void Test(Action test)
        {
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