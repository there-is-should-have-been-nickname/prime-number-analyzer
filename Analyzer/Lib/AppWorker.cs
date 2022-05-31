using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
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

        public static string ConfigFileName = "config";
        public static AppConfig Config = new AppConfig(0, "", "", "", 
            ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black);

        private static void ShowHelp()
        {
            ConsoleManager.ChangeForegroundColor(Config.HelpColor);
            ConsoleManager.Print(new List<string>() { "'help' для вызова помощи" ,
                "'sync m' для синхронного перебора, где m - кол-во тестов",
                "'thread n m' для многопоточного перебора, где n - кол-во потоков, m - кол-во тестов",
                "'delete' для удаления всех созданных текстовых файлов",
                "'clear' для очистки консоли",
                "'exit' для выхода",
                "*В файлах по " + Config.AmountNumbers + " чисел*"
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
                Numbers = FileManager.ReadFile(Config.AmountNumbers, i);

                TimeWatcher.Start();
                var PrimeNumber = Analyzer.FindFirstPrime(Numbers);
                TimeWatcher.Stop();

                if (PrimeNumber == null)
                {
                    ConsoleManager.PrintFail(Config.FailColor,
                        Config.FailResult);
                } else
                {
                    ConsoleManager.PrintSuccess(Config.SuccessColor, 
                        Config.SuccessResult, 
                        PrimeNumber.ToString());
                }
                ConsoleManager.ResetColor();

                ConsoleManager.PrintReport(Config.ReportColor,
                    Config.ReportStart + (i + 1) + ": ", 
                    TimeWatcher.ElapsedMilliseconds.ToString());
                ConsoleManager.ResetColor();
            }
        }

        private static void FinishThread(bool isSuccess, int SuccessNumber = -1)
        {
            if (isSuccess)
            {
                ConsoleManager.PrintSuccess(Config.SuccessColor, 
                    Config.SuccessResult, 
                    SuccessNumber.ToString());
            } else
            {
                ConsoleManager.PrintFail(Config.FailColor, 
                    Config.FailResult);
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
                Numbers = FileManager.ReadFile(Config.AmountNumbers, i);

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

                ConsoleManager.PrintReport(Config.ReportColor,
                    Config.ReportStart + (i + 1) + ": ",
                    TimeWatcher.ElapsedMilliseconds.ToString());
                ConsoleManager.ResetColor();
            }
        }

        private static void GetConfig()
        {
            string text = FileManager.ReadConfigFile(ConfigFileName);
            Config = JsonSerializer.Deserialize<AppConfig>(text);

        }

        public static void Start()
        {
            GetConfig();
            ShowHelp();
            ConsoleManager.Print(new List<string>() { "" });

            string command = ConsoleManager.GetCommand();

            while (command != "exit")
            {    
                
                if (command.StartsWith("sync"))
                {
                    var Params = GetParameters(command);
                    var TestNumber = Params[0];

                    FileManager.CreateFiles(TestNumber, Config.AmountNumbers);
                    ExactSyncTests(TestNumber);
                    
                } else if (command.StartsWith("thread"))
                {
                    var Params = GetParameters(command);
                    var ThreadNumber = Params[0];
                    var TestNumber = Params[1];

                    FileManager.CreateFiles(TestNumber, Config.AmountNumbers);
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
