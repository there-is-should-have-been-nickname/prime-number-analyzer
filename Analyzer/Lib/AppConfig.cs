using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
    public class AppConfig
    {
        public int AmountNumbers { get; }

        public string ReportStart { get; }
        public string FailResult { get; }
        public string SuccessResult { get; }

        public ConsoleColor ReportColor { get; }
        public ConsoleColor FailColor { get; }
        public ConsoleColor SuccessColor { get; }
        public ConsoleColor HelpColor { get; }

        public AppConfig(int amountNumbers, string reportStart, string failResult, string successResult,
            ConsoleColor reportColor, ConsoleColor failColor, ConsoleColor successColor, ConsoleColor helpColor) {

            AmountNumbers = amountNumbers;
            ReportStart = reportStart;
            FailResult = failResult;
            SuccessResult = successResult;
            ReportColor = reportColor;
            FailColor = failColor;
            SuccessColor = successColor;
            HelpColor = helpColor;
        }
    }
}
