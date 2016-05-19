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
    public partial class BrowserMenu : UserControl
    {
        public BrowserMenu()
        {
            InitializeComponent();
            Disposed += BrowserMenu_Disposed;
        }

        private void BrowserMenu_Disposed(object sender, EventArgs e)
        {
            timer?.Dispose();
        }

        Timer timer;
        public int Page = 1, PageSize = 100;
        public bool Working = false;
        int count = 0;

        public Func<int> CountFunc = () => 0;
        public Action Act = () => { };
        public Func<bool> ChangeFocus = () => { return true; };

        private void button1_Click(object sender, EventArgs e)
        {
            loadData(false);
        }

        void loadData(bool auto)
        { 
            if (Working) return;
            Working = true;

            Act();
            count = CountFunc(); // this can be wrong in high loads

            if (!auto || (auto && !textBox2.Focused)) textBox2.Text = Page.ToString();
            if (label1.Text != count.ToString()) label1.Text = count.ToString();

            if (count > PageSize)
            {
                if (Page > 1) buttonFirst.Enabled = buttonPrevious.Enabled = true;
                else buttonFirst.Enabled = buttonPrevious.Enabled = false;

                if (Page == pages) buttonNext.Enabled = buttonLast.Enabled = false;
                else buttonNext.Enabled = buttonLast.Enabled = true;
            }
            else buttonFirst.Enabled = buttonPrevious.Enabled = buttonNext.Enabled = buttonLast.Enabled = false;

            if (!auto) ChangeFocus();
        }

        private void BrowserMenu_Load(object sender, EventArgs e)
        {
            button1_Click(null, null);

            timer = new Timer() { Interval = 2000, Enabled = true };
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!checkBox1.IsDisposed && checkBox1.Checked) loadData(true);
        }

        private void buttonFirst_Click(object sender, EventArgs e)
        {
            Page = 1;
            button1_Click(null, null);
        }

        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            Page--;
            button1_Click(null, null);
        }

        private void textBox2_MouseClick(object sender, MouseEventArgs e)
        {
            textBox2.SelectAll();
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            Page++;
            button1_Click(null, null);
        }

        private void buttonLast_Click(object sender, EventArgs e)
        {
            Page = pages;
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

                    Page = int.Parse(textBox2.Text);
                    button1_Click(null, null);
                }
                else textBox2.Text = Page.ToString();
            }
        }

        int pages => (count / PageSize) + (count % PageSize > 0 ? 1 : 0);
    }
}
