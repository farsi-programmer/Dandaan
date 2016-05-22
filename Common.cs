using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dandaan
{
    class Common
    {
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
