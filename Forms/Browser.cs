using System;
using System.Reflection;
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
    public abstract partial class Browser<T> : Form where T : class
    {
        public Browser()
        {
            InitializeComponent();

            FormClosing += Browser_FormClosing;

            DandaanAttribute = Reflection.GetDandaanAttribute(typeof(T));

            PropertyInfos = typeof(T).GetProperties();
        }

        private void Browser_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer?.Dispose();
            thread?.Abort();
        }

        Editor<T> editor = null;
        Timer timer;
        int count = 0;

        protected DandaanAttribute DandaanAttribute;
        protected PropertyInfo[] PropertyInfos;
        protected System.Threading.Thread thread = null;
        protected int Page = 1, PageSize = 100;
        protected Func<int> CountFunc = SQL.Count<T>;
        protected abstract void Act();

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            if (buttonRefresh.Enabled) loadData(Page);
        }

        private void loadData(int page)
        {
            disable();
            Page = page;
            if (textBox2.Text != Page.ToString()) textBox2.Text = Page.ToString();

            Act();
            count = CountFunc(); // this can be wrong under high load
            if (label1.Text != count.ToString()) label1.Text = count.ToString();
        }

        protected void Enable()
        {
            if (count > PageSize)
            {
                if (Page > 1) buttonFirst.Enabled = buttonPrevious.Enabled = true;
                else buttonFirst.Enabled = buttonPrevious.Enabled = false;

                if (Page == pages) buttonNext.Enabled = buttonLast.Enabled = false;
                else buttonNext.Enabled = buttonLast.Enabled = true;
            }
            else buttonFirst.Enabled = buttonPrevious.Enabled = buttonNext.Enabled = buttonLast.Enabled = false;

            buttonRefresh.Enabled = textBox2.Enabled = true;

            buttonAdd.Enabled = DandaanAttribute.EnableAdd;
            buttonEdit.Enabled = DandaanAttribute.EnableEdit;
            buttonDelete.Enabled = DandaanAttribute.EnableDelete;
            buttonSearch.Enabled = DandaanAttribute.EnableSearch;
        }

        private void Browser_Load(object sender, EventArgs e)
        {
            Text = DandaanAttribute.Label;

            buttonRefresh_Click(null, null);

            timer = new Timer() { Interval = 3000, Enabled = true };
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (checkBox1.Checked) buttonRefresh_Click(null, null);
        }

        private void buttonFirst_Click(object sender, EventArgs e)
        {
            loadData(1);
        }

        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            loadData(Page - 1);
        }

        private void textBox2_MouseClick(object sender, MouseEventArgs e)
        {
            textBox2.SelectAll();
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            loadData(Page + 1);
        }

        private void buttonLast_Click(object sender, EventArgs e)
        {
            loadData(pages);
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (int)Keys.Return)
            {
                var p = 0;
                if (int.TryParse(textBox2.Text, out p))
                {
                    if (p < 1 || pages == 0) textBox2.Text = "1";
                    else if (p > pages) textBox2.Text = pages.ToString();

                    loadData(int.Parse(textBox2.Text));
                }
                else textBox2.Text = Page.ToString();
            }
        }

        private void disable()
        {
            buttonRefresh.Enabled = buttonFirst.Enabled = buttonLast.Enabled = buttonNext.Enabled
            = buttonPrevious.Enabled = textBox2.Enabled = buttonDelete.Enabled = buttonEdit.Enabled = false;
        }

        public Action DeleteAct = () => { };

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            bool isChecked = checkBox1.Checked;
            if (isChecked) checkBox1.Checked = false;

            if (MessageBox.Show("آیا مطمئن هستید که میخواهید این رکورد را حذف کنید؟",
                Program.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                DeleteAct();

            checkBox1.Checked = isChecked;
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (editor == null || editor.IsDisposed)
            {
                editor = new Editor<T>(PropertyInfos);
                editor.Text = DandaanAttribute.Label;
            }

            ShowForm(ref editor);
        }

        int pages => (count / PageSize) + (count % PageSize > 0 ? 1 : 0);
    }
}
