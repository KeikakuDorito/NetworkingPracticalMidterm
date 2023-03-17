//Server with Multiple clients
/*
  - Recieves updates from multiple clients
  - Creates a msg with updates from all clients
  - Sends to all connected clients
  
  - Keeps track of all connected clients - arrays - lists
  -  Update Interval (50ms - 100ms)

  - When a client connects
    - add the client to a list to connected clients
    - Create a thread (to process recievefrom, recv)

  - Server will send update to all clients
    - iterate through the list of clients
    - send update to every single client
 
  -Problem!!
    - memory access (shared buffer)
    - mutex
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace mutex
{
    class Program
    {

        private static Mutex mutex = new Mutex();

        static void Main(string[] args)
        {

            for (int i = 1; i <2; i++)
            {
                Thread t = new Thread(tFunc);
                t.Name = "T" + i;
                Console.WriteLine(t.Name + " has been created!");
                t.Start();
            }

            Console.ReadLine();
            mutex.Dispose();
        }

        private static void tFunc()
        {
            Console.WriteLine(Thread.CurrentThread.Name +
                " is waiting to use the protected resource");

            if (mutex.WaitOne())
            {
                Console.WriteLine(Thread.CurrentThread.Name
                    + " is using the resource");
                // Where you process/use the shared resource
                Thread.Sleep(3000);

                mutex.ReleaseMutex();

                Console.WriteLine(Thread.CurrentThread.Name
                    + " has released the mutex!");
            }

            else
            {
                Console.WriteLine(Thread.CurrentThread.Name 
                    + " could not get a hold of the mutex!");
            }
        }
    }
}
