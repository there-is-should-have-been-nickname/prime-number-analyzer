using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
    public static class ConsoleManager
    {
        public static void Print(List<string> texts)
        {
            foreach(var text in texts)
            {
                Console.WriteLine(text);
            }
        }

        public static void ChangeForegroundColor(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }

        public static void ResetColor()
        {
            Console.ResetColor();
        }

        public static void Clear()
        {
            Console.Clear();
        }

        public static string GetCommand()
        {
            return Console.ReadLine();
        }

        public static void PrintFail(ConsoleColor color, string text)
        {
            ChangeForegroundColor(color);
            Print(new List<string>() { text });
        }

        public static void PrintSuccess(ConsoleColor color, string text, string num)
        {
            Console.Write(text);
            ChangeForegroundColor(color);
            Print(new List<string>() { num });
        }

        public static void PrintReport(ConsoleColor color, string text, string time)
        {
            Console.Write(text);
            ChangeForegroundColor(color);
            Print(new List<string>() { time });
        }
    }
}
