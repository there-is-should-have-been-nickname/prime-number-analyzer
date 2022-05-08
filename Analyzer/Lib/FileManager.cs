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
        public static List<int> ReadFile(string path)
        {
            var Result = new List<int>();
            try
            {
                using (StreamReader sr = new(FILE_PATH + path))
                {
                    string Line = sr.ReadLine();
                    while (Line != null)
                    {   
                        int Number = Convert.ToInt32(Line);
                        Result.Add(Number);
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
    }
}
