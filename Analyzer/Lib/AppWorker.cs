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
        public static bool IsFinish = false;
        public static Mutex mutexObj = new();
        public static List<int> Numbers = new List<int>();

        public readonly static string ReportStart = "Time test #";
        public readonly static string FailResult = "There is no prime number";
        public readonly static string SuccessResult = "The first prime number is ";

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

                Console.WriteLine((PrimeNumber == null) ? FailResult : SuccessResult + PrimeNumber);
                Console.WriteLine(ReportStart + (i + 1) + ": " + TimeWatcher.ElapsedMilliseconds);
            }
        }

        private static void FinishThread(string message)
        {
            Console.WriteLine(message);
            mutexObj.ReleaseMutex();
            IsFinish = true;
        }

        private static void CheckArray(object? obj)
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
                    //var thread = Thread.CurrentThread;
                    //Console.WriteLine("Current thread " + thread.Name);
                    var Number = Numbers[Index];
                    //Console.WriteLine("Number: " + Number);

                    var isPrime = Analyzer.IsPrime(Number);
                    if (isPrime)
                    {
                        FinishThread(SuccessResult + Number);
                        return;
                    }
                    ++Index;
                    mutexObj.ReleaseMutex();
                }
                else
                {
                    FinishThread(FailResult);
                    return;
                }
            }

        }

        private static void ExactMultithreadTests(int threadNumber, int number)
        {
            for (int i = 0; i < number; ++i)
            {
                Index = 0;
                IsFinish = false;

                List<Thread> threads = new List<Thread>();

                Stopwatch TimeWatcher = new Stopwatch();

                for (int j = 0; j < threadNumber; j++)
                {
                    var thread = new Thread(new ParameterizedThreadStart(CheckArray));
                    thread.Name = (j + 1).ToString();
                    threads.Add(thread);

                }

                TimeWatcher.Start();
                threads.ForEach(T => T.Start());

                foreach (Thread thread in threads)
                {
                    if (thread.IsAlive)
                    {
                        thread.Join();
                    }
                }
                TimeWatcher.Stop();
                Console.WriteLine(ReportStart + (i + 1) + ": " + TimeWatcher.ElapsedMilliseconds);
            }
        }

        public static void Start()
        {

            Numbers = FileManager.ReadFile("numbers1000.txt");
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

                    
                    ExactMultithreadTests(ThreadNumber, TestNumber);
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
            }
        }
    }
}
