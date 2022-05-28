using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Lib
{
    public static class AppWorker
    {
        public static int Index = 0;
        public static Mutex mutexObj = new();
        public static List<int> Numbers = new List<int>();
        public static bool IsFinish = false;
        private static void ShowHelp()
        {
            Console.WriteLine("'help' для вызова помощи");
            Console.WriteLine("'sync m' для синхронного перебора, где m - кол-во тестов");
            Console.WriteLine("'thread n m' для многопоточного перебора, где n - кол-во потоков, m - кол-во тестов");
            Console.WriteLine("'exit' для выхода");
        }

        private static List<int> GetParameters(string inputStr)
        {
            var Result = new List<int>();
            var Parts = inputStr.Split(' ');
            for (int i = 1; i < Parts.Count(); ++i) {
                Result.Add(Convert.ToInt32(Parts[i]));
            }

            return Result;
        }

        private static void ExactSyncTests(int number)
        {
            var TimeWatcher = new Stopwatch();
            for (int i = 0; i < number; ++i)
            {
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

                Console.WriteLine("Time test #" + (i + 1) + ": " + TimeWatcher.ElapsedMilliseconds);
            }
        }

        public static void CheckArray(object? obj)
        {
            while (true)
            {
                mutexObj.WaitOne();

                if (IsFinish)
                {
                    mutexObj.ReleaseMutex();

                    return;
                }

                if (Index < Numbers.Count)
                {
                    var thread = Thread.CurrentThread;
                    Console.WriteLine("Current thread " + thread.Name);

                    var Number = Numbers[Index];
                    Console.WriteLine("Number: " + Number);

                    var isPrime = Analyzer.IsPrime(Number);
                    if (isPrime)
                    {
                        Console.WriteLine("\nThe first prime number is " + Number);
                        mutexObj.ReleaseMutex();

                        return;
                    }
                    ++Index;
                    mutexObj.ReleaseMutex();
                } else
                {
                    Console.WriteLine("There is no prime number");
                    mutexObj.ReleaseMutex();
                    IsFinish = true;
                    return;
                }
            }

        }

        public static void Start()
        {

            Numbers = FileManager.ReadFile("numbers10.txt");
            ShowHelp();
            Console.WriteLine();

            string command = Console.ReadLine();

            while (command != "exit")
            {    
                
                if (command.StartsWith("sync"))
                {
                    var Params = GetParameters(command);
                    var TestNumber = Params[0];

                    ExactSyncTests(TestNumber);
                    
                } else if (command.StartsWith("thread"))
                {
                    var Params = GetParameters(command);
                    var ThreadNumber = Params[0];
                    var TestNumber = Params[1];

                    Index = 0;

                    for (int i = 0; i < ThreadNumber; ++i)
                    {
                        var thread = new Thread(new ParameterizedThreadStart(CheckArray));
                        thread.Name = (i + 1).ToString();
                        thread.Start();
                    }
                }
                else if (command == "help")
                {
                    ShowHelp();
                } else 
                {
                    Console.WriteLine("Unknown command");
                }

                Console.WriteLine();
                command = Console.ReadLine();

                //    case "thread":
                //        Console.WriteLine("Введите число потоков (от 1 до 10)");

                //        string InputNumber = Console.ReadLine();
                //        int Number = Convert.ToInt32(InputNumber);
                //        Index = 0;

                //        for (int i = 0; i < Number; ++i)
                //        {
                //            var thread = new Thread(new ParameterizedThreadStart(CheckArray));
                //            thread.Name = (i + 1).ToString();
                //            thread.Start(Numbers);
                //        }

                //        break;


            }
        }
    }
}
