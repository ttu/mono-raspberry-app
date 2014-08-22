using Microsoft.Owin.Hosting;
using Nancy.Hosting.Self;
using System;
using System.Linq;
using System.Threading;

namespace NancySelfHost
{
    // https://github.com/NancyFx/Nancy/wiki/Hosting-Nancy-with-Nginx-on-Ubuntu

    // https://github.com/NancyFx/Nancy/wiki/Hosting-nancy-with-owin

    internal class Program
    {
        private static void Main(string[] args)
        {
            var url = "http://+:8080";
            OwinHost(args, url);
        }

        private static void OwinHost(string[] args, string url)
        {
            using (WebApp.Start<Startup>(url))
            {
                WaitForInput(args, url);
            }
        }

        private static void NgixHost(string[] args, string url)
        {
            // initialize an instance of NancyHost (found in the Nancy.Hosting.Self package)
            var config = new HostConfiguration();
            config.UrlReservations.CreateAutomatically = true;

            var host = new NancyHost(config, new Uri(url));

            try
            {
                host.Start();

                WaitForInput(args, url);
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

        private static void WaitForInput(string[] args, string url)
        {
            Console.WriteLine("Running on {0}", url);
            Console.WriteLine("Press any key to exit");

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
    }
}