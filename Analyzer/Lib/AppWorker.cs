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
        public static int AmountNumbers = 100;
        

        public readonly static string ReportStart = "Time test #";
        public readonly static string FailResult = "There is no prime number";
        public readonly static string SuccessResult = "The first prime number is ";

        public readonly static ConsoleColor ReportColor = ConsoleColor.DarkYellow;
        public readonly static ConsoleColor FailColor = ConsoleColor.Red;
        public readonly static ConsoleColor SuccessColor = ConsoleColor.Green;
        public readonly static ConsoleColor HelpColor = ConsoleColor.Blue;

        private static void ShowHelp()
        {
            ConsoleManager.ChangeForegroundColor(HelpColor);
            ConsoleManager.Print(new List<string>() { "'help' для вызова помощи" ,
                "'sync m' для синхронного перебора, где m - кол-во тестов",
                "'thread n m' для многопоточного перебора, где n - кол-во потоков, m - кол-во тестов",
                "'delete' для удаления всех созданных текстовых файлов",
                "'clear' для очистки консоли",
                "'exit' для выхода",
                "*В файлах по " + AmountNumbers + " чисел*"
            });
            ConsoleManager.ResetColor();
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
                Numbers = FileManager.ReadFile(AmountNumbers, i);

                TimeWatcher.Start();
                var PrimeNumber = Analyzer.FindFirstPrime(Numbers);
                TimeWatcher.Stop();

                if (PrimeNumber == null)
                {
                    ConsoleManager.PrintFail(FailColor, FailResult);
                } else
                {
                    ConsoleManager.PrintSuccess(SuccessColor, SuccessResult, PrimeNumber.ToString());
                }
                ConsoleManager.ResetColor();

                ConsoleManager.PrintReport(ReportColor, 
                    ReportStart + (i + 1) + ": ", 
                    TimeWatcher.ElapsedMilliseconds.ToString());
                ConsoleManager.ResetColor();
            }
        }

        private static void FinishThread(bool isSuccess, int SuccessNumber = -1)
        {
            if (isSuccess)
            {
                ConsoleManager.PrintSuccess(SuccessColor, SuccessResult, SuccessNumber.ToString());
            } else
            {
                ConsoleManager.PrintFail(FailColor, FailResult);
            }
            ConsoleManager.ResetColor();
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
                        FinishThread(true, Number);
                        return;
                    }
                    ++Index;
                    mutexObj.ReleaseMutex();
                }
                else
                {
                    FinishThread(false);
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
                Numbers = FileManager.ReadFile(AmountNumbers, i);

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

                ConsoleManager.PrintReport(ReportColor,
                    ReportStart + (i + 1) + ": ",
                    TimeWatcher.ElapsedMilliseconds.ToString());
                ConsoleManager.ResetColor();
            }
        }

        public static void Start()
        {
            ShowHelp();
            ConsoleManager.Print(new List<string>() { "" });

            string command = ConsoleManager.GetCommand();

            while (command != "exit")
            {    
                
                if (command.StartsWith("sync"))
                {
                    var Params = GetParameters(command);
                    var TestNumber = Params[0];

                    FileManager.CreateFiles(TestNumber, AmountNumbers);
                    ExactSyncTests(TestNumber);
                    
                } else if (command.StartsWith("thread"))
                {
                    var Params = GetParameters(command);
                    var ThreadNumber = Params[0];
                    var TestNumber = Params[1];

                    FileManager.CreateFiles(TestNumber, AmountNumbers);
                    ExactMultithreadTests(ThreadNumber, TestNumber);
                }
                else if (command == "delete")
                {
                    FileManager.DeleteFiles();
                }
                else if (command == "clear")
                {
                    ConsoleManager.Clear();
                }
                else if (command == "help")
                {
                    ShowHelp();
                } else 
                {
                    ConsoleManager.Print(new List<string>() { "Unknown command" });
                }

                ConsoleManager.Print(new List<string>() { "" });
                command = ConsoleManager.GetCommand();
            }
        }
    }
}
