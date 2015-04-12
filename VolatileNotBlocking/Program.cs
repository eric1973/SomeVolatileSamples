using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VolatileNotBlocking
{
    class Test
    {
        internal volatile bool loop = true;
    }

    class Program
    {
        static void Main(string[] args)
        {
            Test test = new Test();


            new Thread(() =>
            {
                //Thread.MemoryBarrier();
                test.loop = false;
                //Thread.MemoryBarrier();
            }).Start();

            //Thread.MemoryBarrier();
            while (test.loop == true)
            {
                //Thread.MemoryBarrier();
            }

            Console.Write("Gotcha!");
            Console.Read();
        }
    }
}
