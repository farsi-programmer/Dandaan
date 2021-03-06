﻿using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Dandaan
{
    public static class Common
    {
        public static void Iter<T>(this IEnumerable<T> source, Action<T> act)
        {
            foreach (T element in source) act(element);
        }

        [DllImport("user32.dll")]
        static extern bool SetCaretPos(int X, int Y);

        [DllImport("user32.dll")]
        static extern bool GetCaretPos(out Point lpPoint);

        public static Match Match(string input, string pattern) => Regex.Match(input, pattern, RegexOptions.IgnoreCase);

        public static bool IsMatch(string input, string pattern) => Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase);

        public static Action Action(Action act) => act;

        public static Thread Thread(Action act)
        {
            return new Thread(() =>
            {
                try
                {
                    act();
                }
                catch (ThreadAbortException) { }
                catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached)
                {
                    while (ex.InnerException != null) ex = ex.InnerException;

                    DB.Log(ex.ToString());

                    new Forms.Message(Program.Title, "برنامه با مشکل مواجه شده است:‏\r\n" + ex).ShowDialog();
                }
            });
        }
    }
}
