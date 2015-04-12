using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadSafeField
{
    class Test
    {
        object locker = new object();
        List<TimeSpan> intervals; // shared on multi threads
        int lastCount;

        ConcurrentDictionary<int, Lazy<List<TimeSpan>>> dict = 
            new ConcurrentDictionary<int,Lazy<List<TimeSpan>>>();

        internal Lazy<List<TimeSpan>> GetIntervals(int count)
        {
            Lazy<List<TimeSpan>> listFactory;

            listFactory = dict.GetOrAdd(count, (key) =>
            {
                //Console.WriteLine("valueFactory for count '{0}'.", key);
                return new Lazy<List<TimeSpan>>(
                    () => this.CalculateIntervals(count), 
                    isThreadSafe:true);
            });

            return listFactory;
        }

        List<TimeSpan> CalculateIntervals(int count)
        {
            //Console.WriteLine("Calculate list for count '{0}' ...", count);
            return new List<TimeSpan>(count);
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            List<Thread> threadlist = new List<Thread>();
            Test tester  =new Test();

            for (int i = 0; i < 1000000; i++)
            {
                int threadlocalCount = i % 100000; // range: 0 to 9999

                Thread worker = new Thread(() =>
                {
                    //Console.WriteLine("ThreadId: {0} procesing count '{1}'",
                    //    Thread.CurrentThread.ManagedThreadId,
                    //    threadlocalCount);

                    var intervals = tester.GetIntervals(threadlocalCount);

                    if (threadlocalCount != intervals.Value.Capacity)
                    {
                        Console.WriteLine(
                            string.Format(
                            "count {0} != retrieved list count {1}.", 
                            threadlocalCount, 
                            intervals.Value.Capacity));
                    }
                });
                
                threadlist.Add(worker);
                worker.Start();
            }

            // Wait for all worker 
            Console.WriteLine(string.Format("Wait for all {0} worker...", threadlist.Count));

            foreach (var worker in threadlist)
	        {
		        worker.Join();
	        }

            Console.WriteLine("All worker finished.");
            Console.ReadLine();
        }

        
    }
}
