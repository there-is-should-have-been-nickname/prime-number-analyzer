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
        public static List<List<int>> Lists = new List<List<int>>();
        public static List<Thread> Threads = new List<Thread>();

        public static string ConfigFileName = "config";
        public static AppConfig Config = new AppConfig(0, "", "", "", 
            ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black);

        private static void GetConfig()
        {
            string text = FileManager.ReadConfigFile(ConfigFileName);
            Config = JsonSerializer.Deserialize<AppConfig>(text);

        }
        private static void ShowHelp()
        {
            ConsoleManager.ChangeForegroundColor(Config.HelpColor);
            ConsoleManager.Print(new List<string>() { "'help' для вызова помощи" ,
                "'sync m' для синхронного перебора, где m - кол-во тестов",
                "'thread n m' для многопоточного перебора, где n - кол-во потоков, m - кол-во тестов",
                "'parallel m' для параллельного перебора, где m - кол-во тестов",
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

        private static void ProcessNotPrimeNumber(int? number, Stopwatch timeWatcher, int testNumber)
        {
            if (number == null)
            {
                ConsoleManager.PrintFail(Config.FailColor,
                    Config.FailResult);
            }
            else
            {
                ConsoleManager.PrintSuccess(Config.SuccessColor,
                    Config.SuccessResult,
                    number.ToString());
            }
            ConsoleManager.ResetColor();

            ConsoleManager.PrintReport(Config.ReportColor,
                Config.ReportStart + testNumber + ": ",
                timeWatcher.ElapsedMilliseconds.ToString());
            ConsoleManager.ResetColor();
        }

        private static void ProcessNotPrimeNumber(int? number, int testNumber)
        {
            if (number == null)
            {
                ConsoleManager.PrintFail(Config.FailColor,
                    Config.FailResult);
            }
            else
            {
                ConsoleManager.PrintSuccess(Config.SuccessColor,
                    Config.SuccessResult,
                    number.ToString());
            }
            ConsoleManager.ResetColor();
        }

        private static void ExactSyncTests(int number)
        {
            double SumTime = 0;
            var TimeWatcher = new Stopwatch();
            for (int i = 0; i < number; ++i)
            {
                Numbers = FileManager.ReadFile(Config.AmountNumbers, i);

                TimeWatcher.Reset();
                TimeWatcher.Start();
                var NotPrimeNumber = Analyzer.FindFirstNotPrime(Numbers);
                TimeWatcher.Stop();
                SumTime += TimeWatcher.ElapsedMilliseconds;

                ProcessNotPrimeNumber(NotPrimeNumber, TimeWatcher, i + 1);
            }
            ConsoleManager.Print(new List<string>() { (SumTime / number).ToString() });
        }
        

        //private static void CheckArray(object? obj)
        //{
        //    while (true)
        //    {
        //        mutexObj.WaitOne();

        //        if (IsFinish)
        //        {
        //            mutexObj.ReleaseMutex();
        //            return;
        //        }

        //        if (Index < Numbers.Count)
        //        {
        //            //var thread = Thread.CurrentThread;
        //            //Console.WriteLine("Current thread " + thread.Name);
        //            var Number = Numbers[Index];
        //            //Console.WriteLine("Number: " + Number);

        //            var isNotPrime = Analyzer.IsNotPrime(Number);
        //            if (isNotPrime)
        //            {
        //                FinishThread(true, Number);
        //                return;
        //            }
        //            ++Index;
        //            mutexObj.ReleaseMutex();
        //        }
        //        else
        //        {
        //            FinishThread(false);
        //            return;
        //        }
        //    }
        //}

        private static void CheckArray(object? obj)
        {
            int ListInd = (int)obj;
            var NotPrimeNumber = Analyzer.FindFirstNotPrime(Lists[ListInd]);
            //ProcessNotPrimeNumber(NotPrimeNumber, ListInd);

            if (NotPrimeNumber != null)
            {
                FinishThreads(Threads);
            }

        }
        //private static void FinishThread(bool isSuccess, int SuccessNumber = -1)
        //{
        //    if (isSuccess)
        //    {
        //        ConsoleManager.PrintSuccess(Config.SuccessColor,
        //            Config.SuccessResult,
        //            SuccessNumber.ToString());
        //    }
        //    else
        //    {
        //        ConsoleManager.PrintFail(Config.FailColor,
        //            Config.FailResult);
        //    }
        //    ConsoleManager.ResetColor();
        //    mutexObj.ReleaseMutex();
        //    IsFinish = true;
        //}

        private static void FinishThreads(List<Thread> threads)
        {
            foreach(var thread in threads)
            {
                thread.Interrupt();
            }
        }

        //private static void ExactMultithreadTests(int threadNumber, int number)
        //{
        //    double SumTime = 0;
        //    for (int i = 0; i < number; ++i)
        //    {
        //        Index = 0;
        //        IsFinish = false;
        //        Numbers = FileManager.ReadFile(Config.AmountNumbers, i);

        //        Stopwatch TimeWatcher = new Stopwatch();
        //        TimeWatcher.Reset();
        //        TimeWatcher.Start();

        //        List<Thread> threads = new List<Thread>();


        //        for (int j = 0; j < threadNumber; j++)
        //        {
        //            var thread = new Thread(new ParameterizedThreadStart(CheckArray));
        //            thread.Name = (j + 1).ToString();
        //            threads.Add(thread);

        //        }

        //        threads.ForEach(T => T.Start());

        //        foreach (Thread thread in threads)
        //        {
        //            if (thread.IsAlive)
        //            {
        //                thread.Join();
        //            }
        //        }
        //        TimeWatcher.Stop();

        //        SumTime += TimeWatcher.ElapsedMilliseconds;
        //        ConsoleManager.PrintReport(Config.ReportColor,
        //            Config.ReportStart + (i + 1) + ": ",
        //            TimeWatcher.ElapsedMilliseconds.ToString());
        //        ConsoleManager.ResetColor();
        //    }
        //    ConsoleManager.Print(new List<string>() { (SumTime / number).ToString() });
        //}

        private static List<List<int>> GetListOfLists(int listNumber, List<int> numbers)
        {
            var result = new List<List<int>>();
            var numberInOneList = numbers.Count / listNumber;
            var currentNumbersInList = 0;
            var currentList = new List<int>();
            var ind = 0;

            while (ind < numbers.Count)
            {
                if (currentNumbersInList < numberInOneList)
                {
                    currentList.Add(numbers[ind]);
                    ++currentNumbersInList;
                    ++ind;
                } else
                {
                    result.Add(currentList);
                    currentList = new List<int>();
                    currentNumbersInList = 0;
                }
            }
            if (currentNumbersInList != 0 && numbers.Count % listNumber != 0)
            {
                foreach (var elem in currentList)
                {
                    result[result.Count - 1].Add(elem);
                }
            }
            if (currentNumbersInList != 0 && numbers.Count % listNumber == 0)
            {
                result.Add(currentList);
            }

            return result;
        }

        private static void ExactMultithreadTests(int threadNumber, int number)
        {
            double SumTime = 0;
            
            for (int i = 0; i < number; ++i)
            {
                Index = 0;
                IsFinish = false;
                Numbers = FileManager.ReadFile(Config.AmountNumbers, i);

                Lists = GetListOfLists(threadNumber, Numbers);

                Stopwatch TimeWatcher = new Stopwatch();
                TimeWatcher.Reset();
                TimeWatcher.Start();

                Threads = new List<Thread>();

                for (int j = 0; j < threadNumber; j++)
                {
                    var thread = new Thread(new ParameterizedThreadStart(CheckArray));
                    thread.Name = (j + 1).ToString();
                    Threads.Add(thread);
                    thread.Start(j);

                }

                foreach (Thread thread in Threads)
                {
                    if (thread.IsAlive)
                    {
                        thread.Join();
                    }
                }
                TimeWatcher.Stop();

                SumTime += TimeWatcher.ElapsedMilliseconds;
                ConsoleManager.PrintReport(Config.ReportColor,
                    Config.ReportStart + (i + 1) + ": ",
                    TimeWatcher.ElapsedMilliseconds.ToString());
                ConsoleManager.ResetColor();
            }
            ConsoleManager.Print(new List<string>() { (SumTime / number).ToString() });
        }

        private static void ExactParallelTests(int number)
        {
            var TimeWatcher = new Stopwatch();
            double SumTime = 0;
            for (int i = 0; i < number; ++i)
            {
                Numbers = FileManager.ReadFile(Config.AmountNumbers, i);
                int? NotPrimeNumber = null;
                TimeWatcher.Reset();
                TimeWatcher.Start();
                Parallel.ForEach(Numbers, (number, state) =>
                {
                    if (Analyzer.IsNotPrime(number))
                    {
                        NotPrimeNumber = number;
                        state.Break();
                    }
                });
                TimeWatcher.Stop();
                SumTime += TimeWatcher.ElapsedMilliseconds;

                ProcessNotPrimeNumber(NotPrimeNumber, TimeWatcher, i + 1);
            }
            ConsoleManager.Print(new List<string>() { (SumTime / number).ToString() });
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
                else if (command.StartsWith("parallel"))
                {
                    var Params = GetParameters(command);
                    var TestNumber = Params[0];

                    FileManager.CreateFiles(TestNumber, Config.AmountNumbers);
                    ExactParallelTests(TestNumber);
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
