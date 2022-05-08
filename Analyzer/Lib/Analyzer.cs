using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
    static class Analyzer
    {
        private static bool IsPrime(int number)
        {
            for (int i = 2; i <= Math.Sqrt(number); ++i)
            {
                if (number % i == 0)
                {
                    return false;
                }
            }

            return true;
        }

        public static int FindFirstPrime(List<int> Numbers)
        {
            foreach (var Num in Numbers)
            {
                if (IsPrime(Num))
                {
                    return Num;
                }
            }
            return -1;
        }
    }
}
