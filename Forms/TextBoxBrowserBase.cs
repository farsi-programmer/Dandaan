using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dandaan.Forms
{
    public partial class TextBoxBrowserBase<T> : Browser<T> where T : class
    {
        protected Func<IEnumerable<string>> LinesFunc;

        protected TextBoxBase TextBox;

        public TextBoxBrowserBase()
        {
            InitializeComponent();

            // testing
            //browserMenu1.CountFunc = () => 1000;
            //var x = new List<object>(1000);
            //for (int i = 0; i < 1000; i++) x.Add(i);
            //LinesFunc = () => { return x.Skip((browserMenu1.Page - 1) * browserMenu1.PageSize).Take(browserMenu1.PageSize).Select((k) => (object)(((int)k) + DateTime.Now.Second)).ToArray(); };

            LinesFunc = () =>
            SQL.Select<T>(Page, PageSize)
            .Select((row) =>
            {
                var sb = new StringBuilder();
                foreach (var item in PropertyInfos)
                {
                    var obj = item.GetValue(row);

                    // replacing new lines is specially necessary if we want to get the id for edit or delete
                    if (obj is string) obj = Regex.Replace(obj as string, "[\r\n]+", " ");

                    sb.Append(obj + "\t");
                }
                return sb.ToString();
            });

            // this is if we want to be able to undo, but we don't, we have it in db
            //str = Regex.Replace(log.Message, Regex.Escape(@"\r\n"), @"\\r\\n");
            //sb.Append($"{log.Id}\t{log.DateTime}\t{Regex.Replace(str, "\r\n", @"\r\n")}\r\n");
        }

        protected override void LoadAct()
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

                if (TextBox is RichTextBox)
                {
                    rtb = new RichTextBox() { Location = new Point(Width, Height) };
                    rtb.Hide();
                    Invoke(() => Controls.Add(rtb));
                }

                bool odd = false;
                foreach (var item in LinesFunc())
                {
                    var s = TextBox is RichTextBox ? item.Replace("\r", "") + "\n\n" : item + "\r\n\r\n";

                    sb.Append(s);

                    if (TextBox is RichTextBox)
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
                    if (str == "") TextBox.Text = "There are no records.";
                    else if (TextBox.Text != str)
                    {
                        var beforeSelection = TextBox.Text.Substring(0, TextBox.SelectionStart);
                            //TextBox.Hide();

                            if (TextBox is RichTextBox) (TextBox as RichTextBox).Rtf = rtb.Rtf;
                        else TextBox.Text = str;

                        scroll(TextBox, beforeSelection);
                            //TextBox.Show();
                        }

                    if (TextBox is RichTextBox) Controls.Remove(rtb);

                    Enable();
                });
            });

            thread.Start();
#endif
        }

        protected override void DeleteAct()
        {
            ;
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
            //Common.GetCaretPos(out point);
            //Common.SetCaretPos(point.X, point.Y);            
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
    }
}
