using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
    public static class FileManager
    {
        public readonly static string FILE_PATH = "C:\\Users\\ACER\\Desktop\\Projects\\prime-number-analyzer\\Analyzer\\Analyzer\\";
        public readonly static string DEFAULT_NAME = "numbers";
        public readonly static int MAX_VALUE = 10000;

        private static void CreateFile(string path, int amountNumbers)
        {
            var rand = new Random();
            try
            {
                using (StreamWriter sw = new(path))
                {
                    for (int j = 0; j < amountNumbers; ++j)
                    {
                        int RandNumber = rand.Next(1, MAX_VALUE);
                        if (j == amountNumbers - 1)
                        {
                            sw.Write(RandNumber);
                        }
                        else
                        {
                            sw.WriteLine(RandNumber);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static List<int> ReadFile(int amountNumbers, int testNumber)
        {
            var Result = new List<int>();
            try
            {
                using (StreamReader sr = new(FILE_PATH + DEFAULT_NAME + amountNumbers + "_" + (testNumber + 1) + ".txt"))
                {
                    string Line = sr.ReadLine();
                    while (Line != null)
                    {
                        int ParsedNum = 0;
                        bool IsParse = int.TryParse(Line, out ParsedNum);
                        if (IsParse)
                        {
                            Result.Add(ParsedNum);
                        }

                        Line = sr.ReadLine();
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Result;
        }

        public static void CreateFiles(int number, int amountNumbers)
        {
            for (int i = 0; i < number; ++i)
            {
                CreateFile(FILE_PATH + DEFAULT_NAME + amountNumbers + "_" + (i + 1) + ".txt", amountNumbers);
            } 
            
        }

        public static void DeleteFiles()
        {
            string[] DeletedFiles = Directory.GetFiles(FILE_PATH, @"*.txt");
            foreach (string DeletedFile in DeletedFiles)
            {
                File.Delete(DeletedFile);
            }
        }
    }
}
