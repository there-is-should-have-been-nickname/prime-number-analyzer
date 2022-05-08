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
        private static void ShowHelp()
        {
            Console.WriteLine("'help' для вызова помощи");
            Console.WriteLine("'sync' для синхронного перебора");
            Console.WriteLine("'thread n' для многопоточного перебора, где n - кол-во потоков (от 1 до 10)");
            Console.WriteLine("'exit' для выхода");
            Console.WriteLine();
        }
        public static void Start()
        {

            var Numbers = FileManager.ReadFile("numbers100000.txt");
            ShowHelp();

            string command = Console.ReadLine();

            while (command != "exit")
            {    
                var TimeWatcher = new Stopwatch();
                switch (command)
                {
                    case "sync":
                        TimeWatcher.Start();
                        var PrimeNumber = Analyzer.FindFirstPrime(Numbers);
                        TimeWatcher.Stop();
                        if (PrimeNumber == -1)
                        {
                            Console.WriteLine("There is no prime number");
                        }
                        else
                        {
                            Console.WriteLine("The first prime number is " + PrimeNumber);
                        }

                        Console.WriteLine("Sync time: " + TimeWatcher.ElapsedMilliseconds);
                        break;
                    case "thread":
                        break;
                    case "help":
                        ShowHelp();
                        break;
                    default:
                        Console.WriteLine("Unknown command");
                        break;
                }
                
                command = Console.ReadLine();
            }
        }
    }
}
