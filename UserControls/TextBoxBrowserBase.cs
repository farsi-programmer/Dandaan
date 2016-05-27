using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dandaan.UserControls
{
    public partial class TextBoxBrowserBase : Browser
    {
        public Func<IEnumerable<string>> LinesFunc = () => new string[] { };

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
#if using_ef || using_sqlite
            DB.Run((context) =>
            {
                foreach (var item in context.Logs)
                {
                    textBox1.AppendText(item.Id + "\t" + item.DateTime + "\t" + item.Message + "\r\n");
                }
            });
#else
                thread = Common.Thread(() =>
                {
                    var sb = new StringBuilder();
                    RichTextBox rtb = null;

                    if (textBox is RichTextBox)
                    {
                        rtb = new RichTextBox() { Location = new Point(Width, Height) };
                        rtb.Hide();
                        Invoke(() => Controls.Add(rtb));
                    }

                    bool odd = false;
                    foreach (var item in LinesFunc())
                    {
                        var s = textBox is RichTextBox ? item.Replace("\r", "") + "\n\n" : item + "\r\n\r\n";

                        sb.Append(s);

                        if (textBox is RichTextBox)
                        {
                            Invoke(() =>
                            {
                                rtb.AppendText(s);
                                rtb.Select(rtb.Text.Length - s.Length, s.Length - 2);
                                //rtb.SelectionBackColor = Color.FromArgb(0xdc, 0xdc, 0xdc);
                                rtb.SelectionBackColor = odd ? Color.LightCyan : Color.FromArgb(0xcf, 0xff, 0xcf);
                                odd = !odd;
                            });
                        }
                    }

                    var str = sb.ToString();

                    Invoke(() =>
                    {
                        if (str == "") textBox.Text = "There are no records.";
                        else if (textBox.Text != str)
                        {
                            var beforeSelection = textBox.Text.Substring(0, textBox.SelectionStart);
                            //textBox.Hide();

                            if (textBox is RichTextBox) (textBox as RichTextBox).Rtf = rtb.Rtf;
                            else textBox.Text = str;

                            scroll(textBox, beforeSelection);
                            //textBox.Show();
                        }

                        if (textBox is RichTextBox) Controls.Remove(rtb);

                        browserMenu1.enable();
                    });
                });
                
                thread.Start();
#endif
            };
        }

        static void scroll(TextBoxBase textBox, string beforeSelection)
        {
            if (textBox.Text.IndexOf(beforeSelection) == 0)
            {
                textBox.SelectionStart = beforeSelection.Length;
                textBox.ScrollToCaret();
            }
            else textBox.ScrollToCaret();

            //Point point = new Point();
            //GetCaretPos(out point);
            //SetCaretPos(point.X, point.Y);            
        }

        static string getLine(TextBoxBase textBox)
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
