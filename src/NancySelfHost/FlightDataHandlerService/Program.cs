using System;
using System.Linq;
using System.Threading;

namespace FlightDataHandlerService
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var dt = new DataHandlerWrapper();
                Console.WriteLine("Service running a Flight Data Handler");
                Console.WriteLine("Press any key to quit");

                // Under mono if you deamonize a process a Console.ReadLine will cause an EOF
                // so we need to block another way
                if (args.Any(s => s.Equals("-d", StringComparison.CurrentCultureIgnoreCase)))
                {
                    Thread.Sleep(Timeout.Infinite);
                }
                else
                {
                    Console.ReadKey();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}