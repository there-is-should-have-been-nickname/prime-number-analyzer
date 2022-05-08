using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
    public static class AppWorker
    {
        public static void Start()
        {
            string command = Console.ReadLine();

            while (command != "exit")
            {
                var Numbers = FileManager.ReadFile("numbers.txt");
                var TimeWatcher = new Stopwatch();
                if (command == "sync")
                {
                    TimeWatcher.Start();
                    var PrimeNumber = Analyzer.FindFirstPrime(Numbers);
                    TimeWatcher.Stop();
                    if (PrimeNumber == -1)
                    {
                        Console.WriteLine("There is no prime number");
                    } else
                    {
                        Console.WriteLine("The first prime number is " + PrimeNumber);
                    }
                    
                }
                Console.WriteLine("Time: " + TimeWatcher.ElapsedMilliseconds);
                command = Console.ReadLine();
            }
        }
    }
}
