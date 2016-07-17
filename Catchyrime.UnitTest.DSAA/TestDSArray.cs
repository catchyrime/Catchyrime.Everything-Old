using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Catchyrime.Everything;
using Catchyrime.Everything.DSAA;
using Wintellect.PowerCollections;

namespace Catchyrime.UnitTest.DSAA
{
    public static class TestDSArray
    {
        private static void Test_DSArray_RemoveAt__Internal(int cnt, int seed)
        {
            Random rand = new Random(seed);
            DSArray<int> array = new DSArray<int>(Enumerable.Range(0, cnt));
            OrderedSet<int> set = new OrderedSet<int>(Enumerable.Range(0, cnt));
            while (array.Count > 0) {
                int idx = rand.Next(0, array.Count - 1);
                int value = set[idx];
                if (array[idx] != value) {
                    Trace.Assert(false);
                }
                array.RemoveAt(idx);
                set.Remove(value);
            }

        }

        private static void Test_DSArray_RemoveAt()
        {
            Console.WriteLine("[Test] Test_DSArray_RemoveAt");

            Stopwatch watch = Stopwatch.StartNew();
            Random rand = new Random(19965013);
            var tests = from int i in Enumerable.Range(0, 1000)
                        let cnt = rand.Next(1, 1000000)
                        let seed = rand.Next()
                        select new {
                            Count = cnt,
                            Seed = seed
                        };

            int tCase = 0;
            tests.AsParallel().ForAll((t) => {
                Console.WriteLine($"    Case #{Interlocked.Increment(ref tCase)}: Count = {t.Count}, Seed = {t.Seed}");
                Test_DSArray_RemoveAt__Internal(t.Count, t.Seed);
            });
            Console.WriteLine("[Test] Test_DSArray_RemoveAt: " + watch.Elapsed);
        }

        public static void Test_DSArray()
        {
            Test_DSArray_RemoveAt();

            /*
            Stopwatch watch = Stopwatch.StartNew();

            const int MAX = 1000000;
            DSArray<int> array = new DSArray<int>();
            for (int i = 0; i < MAX; i++) {
                array.Add(i);
            }

            var arrayCpy = new DSArray<int>(Enumerable.Range(0, MAX));
            Trace.Assert(Equals(array, arrayCpy));

            int tmp = 0;
            foreach (int i in array) {
                if (i != tmp++) {
                    Console.WriteLine(i);
                }
            }
            Console.WriteLine(tmp);

            int[] array2 = new int[MAX + 1];
            array.CopyTo(array2, 1);
            for (int i = 0; i < MAX; i++) {
                if (array2[i + 1] != i) {
                    Console.WriteLine(i);
                }
            }
            Console.WriteLine(watch.Elapsed);
            */
        }
    }
}
