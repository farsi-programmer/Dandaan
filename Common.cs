using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Dandaan
{
    class Common
    {
        public static Match Match(string input, string pattern)
        {
            return Regex.Match(input, pattern, RegexOptions.IgnoreCase);
        }

        public static bool IsMatch(string input, string pattern)
        {
            return Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase);
        }

        public static Action Action(Action act) => act;

        public static Thread Thread(Action act)
        {
            return new Thread(() =>
            {
                //try
                {
                    act();
                }
                //catch (ThreadAbortException) { }
            });
        }
    }
}
