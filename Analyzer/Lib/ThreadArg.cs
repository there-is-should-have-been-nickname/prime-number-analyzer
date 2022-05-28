using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
    public class ThreadArg
    {
        private List<int> numbers = new List<int>();
        private int start = 0;
        private int end = 0;

        public List<int> Numbers { get { return numbers; } set { numbers = value; } }
        public int Start { get { return start; } set { start = value; } }
        public int End { get { return end; } set { end = value; } }
        

        public ThreadArg(List<int> numbers, int start, int end)
        {
            Numbers = numbers;
            Start = start;
            End = end;
        }
    }
}
