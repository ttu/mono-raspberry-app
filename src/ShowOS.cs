using System;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Running on {0}", Environment.OSVersion);

            Console.WriteLine("Press a key to continue");
            Console.ReadKey(true);
        }
    }
}
