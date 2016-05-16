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
    public partial class Browser : UserControl
    {
        public Browser()
        {
            InitializeComponent();
        }

        public void Close()
        {
            timer?.Dispose();
        }

        private Timer timer;
        private int lastCount = 0, lastPage = 0, page = 1, count = 0, pageSize = 100;
        private bool working = false;

        public Func<int> CountFunc = () => 0;
        public Action<int, int, int, Action<bool>> Act = (i, j, k, l) => { };

        private void button1_Click(object sender, EventArgs e)
        {
            if (working) return;
            working = true;
            count = CountFunc();

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

                    if (page == pages) buttonNext.Enabled = buttonLast.Enabled = false;
                    else buttonNext.Enabled = buttonLast.Enabled = true;
                }
                else buttonFirst.Enabled = buttonPrevious.Enabled = buttonNext.Enabled = buttonLast.Enabled = false;

                Act(count, page, pageSize, (newValue) => working = newValue);
            }
            else working = false;
        }

        private void Browser_Load(object sender, EventArgs e)
        {
            button1_Click(null, null);

            timer = new Timer() { Interval = 2000, Enabled = true };
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (checkBox1.Checked) button1_Click(null, null);
        }

        private void buttonFirst_Click(object sender, EventArgs e)
        {
            page = 1;
            button1_Click(null, null);
        }

        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            page--;
            button1_Click(null, null);
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            page++;
            button1_Click(null, null);
        }

        private void buttonLast_Click(object sender, EventArgs e)
        {
            page = pages;
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
                    else if (p > pages) textBox2.Text = pages.ToString();

                    page = int.Parse(textBox2.Text);
                    button1_Click(null, null);
                }
                else textBox2.Text = page.ToString();
            }
        }

        private int pages => (lastCount / pageSize) + (lastCount % pageSize > 0 ? 1 : 0);
    }
}
