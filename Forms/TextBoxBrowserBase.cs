using System;
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
    public partial class TextBoxBrowserBase : Browser
    {
        public Func<string> TextFunc = () => "";

        public TextBoxBrowserBase()
        {
            InitializeComponent();

            // testing
            //browserMenu1.CountFunc = () => 1000;
            //var x = new List<object>(1000);
            //for (int i = 0; i < 1000; i++) x.Add(i);
            //ArrayFunc = () => { return x.Skip((browserMenu1.Page - 1) * browserMenu1.PageSize).Take(browserMenu1.PageSize).Select((k) => (object)(((int)k) + DateTime.Now.Second)).ToArray(); };
        }

        public Action Act(TextBoxBase textBox)
        {
            return () =>
            {
                new System.Threading.Thread(() =>
                {
                    var str = TextFunc();

                    if (textBox is RichTextBox) str = str.Replace("\r", "");

                    Invoke(new Action(() =>
                    {
                        if (str == "") textBox.Text = "There are no records.";
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
                            if (textBox.Text != str)
                            {
                                var beforeSelection = textBox.Text.Substring(0, textBox.SelectionStart);
                                textBox.Text = str;
                                scroll(textBox, beforeSelection);
                            }
                        }

                        browserMenu1.Working = false;
                    }));
#endif
                }).Start();
            };
        }

        static void scroll(TextBoxBase textBox, string beforeSelection)
        {
            if (textBox.Text.IndexOf(beforeSelection) == 0)
            {
                textBox.SelectionStart = beforeSelection.Length;
                textBox.ScrollToCaret();
            }

            //Point point = new Point();
            //GetCaretPos(out point);
            //SetCaretPos(point.X, point.Y);            
        }

        static string selectionLine(TextBoxBase textBox)
        {
            var b = textBox.Text.Substring(0, textBox.SelectionStart).LastIndexOf("\r\n");
            if (b < 0) b = 0;
            else b += 2;

            var e = textBox.Text.IndexOf("\r\n", textBox.SelectionStart);
            if (e < 0) e = textBox.Text.Length;

            return textBox.Text.Substring(b, e - b);
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetCaretPos(int X, int Y);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool GetCaretPos(out Point lpPoint);
    }
}
