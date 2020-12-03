
using System;

namespace ConsoleAppExtension
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             * Step into name to debug the package's source code 
             */
            var name = new Extension().Name;
            Console.WriteLine(name);

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
