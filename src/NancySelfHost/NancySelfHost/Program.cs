using FlightDataHandler;
using Nancy.Hosting.Self;
using System;
using System.Linq;
using System.Threading;

namespace NancySelfHost
{
    // https://github.com/NancyFx/Nancy/wiki/Hosting-Nancy-with-Nginx-on-Ubuntu

    internal class Program
    {
        // TODO: Dependency Injection
        public static Lazy<IDataHandler> DataHandler = new Lazy<IDataHandler>(() => new DataHandler());

        private static void Main(string[] args)
        {
            var uri = "http://localhost:8888";

            // initialize an instance of NancyHost (found in the Nancy.Hosting.Self package)
            var config = new HostConfiguration();
            config.UrlReservations.CreateAutomatically = true;

            var host = new NancyHost(config, new Uri(uri));

            try
            {
                host.Start();

                Console.WriteLine(uri);

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
            catch (Exception)
            {
                throw;
            }
            finally
            {
                host.Stop();
            }
        }
    }
}