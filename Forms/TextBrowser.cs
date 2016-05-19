using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dandaan.Forms
{
    public partial class TextBrowser : Browser
    {
        public Func<string> TextFunc = () => "";

        public TextBrowser()
        {
            InitializeComponent();

            browserMenu1.ChangeFocus = textBox1.Focus;

            browserMenu1.Act = () =>
            {
                new System.Threading.Thread(() =>
                {
                    var str = TextFunc();

                    Invoke(new Action(() =>
                    {
                        if (str == "") textBox1.Text = "There are no records.";
                        else
                        {
#if using_ef || using_sqlite
            DB.Run((context) =>
            {
                foreach (var item in context.Logs)
                {
                    textBox1.AppendText(item.Id + "\t" + item.DateTime + "\t" + item.Message + "\r\n");
                }
            });
#else
                            if (textBox1.Text != str)
                            {
                                var beforeSelection = textBox1.Text.Substring(0, textBox1.SelectionStart);
                                textBox1.Text = str;
                                scroll(beforeSelection);
                            }
                        }

                        browserMenu1.Working = false;
                    }));
#endif
                }).Start();
            };
        }

        void scroll(string beforeSelection)
        {
            if (textBox1.Text.IndexOf(beforeSelection) == 0)
            {
                textBox1.SelectionStart = beforeSelection.Length;
                textBox1.ScrollToCaret();
            }

            //Point point = new Point();
            //GetCaretPos(out point);
            //SetCaretPos(point.X, point.Y);            
        }

        string selectionLine()
        {
            var b = textBox1.Text.Substring(0, textBox1.SelectionStart).LastIndexOf("\r\n");
            if (b < 0) b = 0;
            else b += 2;

            var e = textBox1.Text.IndexOf("\r\n", textBox1.SelectionStart);
            if (e < 0) e = textBox1.Text.Length;

            return textBox1.Text.Substring(b, e - b);
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetCaretPos(int X, int Y);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool GetCaretPos(out Point lpPoint);
    }
}