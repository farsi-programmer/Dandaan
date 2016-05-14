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
    public partial class TextBrowser : UserControl
    {
        public TextBrowser()
        {
            InitializeComponent();

            CountFunc = () => 0;
            TextFunc = (page, pageSize) => "";
        }

        int lastCount = 0, selectionStart = 0, page = 1, pageSize = 100, lastPage = 0;
        string selection = "";
        bool working = false;
        Timer timer;
        public Func<int> CountFunc { get; set; }
        public Func<int, int, string> TextFunc { get; set; }

        public void Close()
        {
            timer?.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (working) return;
            working = true;
            var count = CountFunc();

            if (count != lastCount || page != lastPage)
            {
                lastCount = count;
                lastPage = page;
                textBox2.Text = page.ToString();
                label1.Text = count.ToString();
                if (count > pageSize)
                {
                    if (page > 1) buttonFirst.Enabled = buttonPrevious.Enabled = true;
                    else buttonFirst.Enabled = buttonPrevious.Enabled = false;

                    if (page == (lastCount / pageSize) + (lastCount % pageSize > 0 ? 1 : 0)) buttonNext.Enabled = buttonLast.Enabled = false;
                    else buttonNext.Enabled = buttonLast.Enabled = true;
                }
                else buttonFirst.Enabled = buttonPrevious.Enabled = buttonNext.Enabled = buttonLast.Enabled = false;
                selection = textBox1.SelectedText;
                selectionStart = textBox1.SelectionStart;

                if (count == 0)
                {
                    textBox1.Text = "There are no records.";
                    working = false;
                }
                else
                {
                    textBox1.Text = "Please wait...";

                    new System.Threading.Thread(() =>
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

                        Invoke(new Action(() =>
                        {
                            textBox1.Text = TextFunc(page, pageSize);

                            if (selection.Length == 0 && selectionStart < textBox1.Text.Length)
                                textBox1.SelectionStart = selectionStart;
                            else
                            {
                                var i = textBox1.Text.IndexOf(selection);
                                if (i > -1) textBox1.Select(i, selection.Length + i < textBox1.Text.Length ? selection.Length : textBox1.Text.Length - i - 1);
                            }

                            textBox1.ScrollToCaret();
                            working = false;
                        }));
                    }).Start();
                }
#endif
            }
            else working = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            page = 1;
            selection = ""; selectionStart = 0;
            button1_Click(null, null);
        }

        private void TextBrowser_Load(object sender, EventArgs e)
        {
            button1_Click(null, null);

            timer = new Timer() { Interval = 2000, Enabled = true };
            timer.Tick += Logger_Tick;
        }

        private void Logger_Tick(object sender, EventArgs e)
        {
            if (checkBox1.Checked) button1_Click(null, null);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            page--;
            selection = ""; selectionStart = 0;
            button1_Click(null, null);
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (int)Keys.Return)
            {
                var p = 0;
                if (int.TryParse(textBox2.Text, out p))
                {
                    if (p < 1) textBox2.Text = "1";
                    else if (p > (lastCount / pageSize) + (lastCount % pageSize > 0 ? 1 : 0)) textBox2.Text = ((lastCount / pageSize) + (lastCount % pageSize > 0 ? 1 : 0)).ToString();

                    page = int.Parse(textBox2.Text);
                    button1_Click(null, null);
                }
                else textBox2.Text = page.ToString();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            page++;
            selection = ""; selectionStart = 0;
            button1_Click(null, null);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            page = (lastCount / pageSize) + (lastCount % pageSize > 0 ? 1 : 0);
            selection = ""; selectionStart = 0;
            button1_Click(null, null);
        }
    }
}