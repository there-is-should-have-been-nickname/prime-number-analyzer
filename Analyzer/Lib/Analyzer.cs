using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
    static class Analyzer
    {
        public static bool IsNotPrime(int number)
        {
            for (int i = 2; i <= Math.Sqrt(number); ++i)
            {
                if (number % i == 0)
                {
                    return true;
                }
            }

            return false;
        }

        public static int? FindFirstNotPrime(List<int> Numbers)
        {
            foreach (var Num in Numbers)
            {
                if (IsNotPrime(Num))
                {
                    return Num;
                }
            }
            return null;
        }
    }
}
