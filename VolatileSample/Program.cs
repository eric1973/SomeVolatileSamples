using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/*
 * 
 * 
 * Instruction: Run in Release Config (Compiler Optimization enabled) without Debugger
 * and the 'Gotcha!' COnsole Write is never reached and the while loop runs forever.
 * Test it in Debug COnfig and it is working, because in Debug Config the compiler
 * optimization is not enabled by default.
 * 
 * Without volatile keyword or Thread.MemoryBarrier() the value of 
 * field 'loop' is chached in a CPU REgister for each of the both Thread
 * and the Main Thread accesses forever the initialized field value(witch is true).
 * volatile or Thread.MemoryBarrier() (which is the some but more verbose)
 * prevents compiler optimization for that field and causes the the caches(CPU Register)
 * values for each thread are set to the last value set in memory. 
 * 
 * */

namespace VolatileSample
{
    class Test
    {
        internal /*volatile*/ bool loop = true;
    }

    class Program
    {
        static void Main(string[] args)
        {
            Test test = new Test();


            new Thread(() => {
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
