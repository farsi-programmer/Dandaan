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
    public partial class Browser<T> : Form where T : class
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
            Thread?.Abort();
        }

        Form addForm;
        Timer timer;
        int count = 0;

        protected DandaanAttribute DandaanAttribute;
        protected PropertyInfo[] PropertyInfos;
        protected System.Threading.Thread Thread;
        protected int Page = 1, PageSize = 100;
        protected Func<int> CountFunc = SQL.Count<T>;
        protected Action LoadAct;
        protected Func<bool> DeleteFunc;
        protected Action<Action> EditAct;
        protected Control View;

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            if (buttonRefresh.Enabled) loadData(Page);
        }

        private void loadData(int page)
        {
            go:
            disable();

            count = CountFunc(); // this can be wrong under high load
            if (label1.Text != count.ToString()) label1.Text = count.ToString();
            if (page > pages) { page = pages; goto go; }

            Page = page;
            if (textBox2.Text != Page.ToString()) textBox2.Text = Page.ToString();

            LoadAct();
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
            View.KeyDown += View_KeyDown;
            View.DoubleClick += View_DoubleClick;

            Text = DandaanAttribute.Label;

            buttonRefresh.PerformClick();

            timer = new Timer() { Interval = 3000, Enabled = true };
            timer.Tick += Timer_Tick;
        }

        private void View_DoubleClick(object sender, EventArgs e)
        {
            buttonEdit.PerformClick();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (checkBox1.Checked) buttonRefresh.PerformClick();
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

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            bool isChecked = checkBox1.Checked;
            if (isChecked) checkBox1.Checked = false;

            var success = DeleteFunc();

            checkBox1.Checked = isChecked;
            if (success) buttonRefresh.PerformClick();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (addForm == null || addForm.IsDisposed)
            {
                addForm = new Form() { AutoScroll = true, Text = DandaanAttribute.Label };
                var editor = new UserControls.Editor<T>(PropertyInfos, addForm, acceptAct: acceptAct);
                addForm.Controls.Add(editor);
                addForm.ClientSize = editor.ClientSize;
            }

            ShowForm(ref addForm, false);
        }

        Action acceptAct => () =>
        {
            if (Visible) buttonRefresh.PerformClick();
            else foreach (var item in Application.OpenForms)
                    if (item is Browser<T>)
                        (item as Browser<T>).buttonRefresh.PerformClick();
        };

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            bool isChecked = checkBox1.Checked;
            if (isChecked) checkBox1.Checked = false;

            EditAct(acceptAct);

            checkBox1.Checked = isChecked;
        }

        int pages => (count / PageSize) + (count % PageSize > 0 ? 1 : 0);

        private void View_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete) buttonDelete.PerformClick();
            else if (e.KeyCode == Keys.Insert) buttonAdd.PerformClick();
            else if (e.KeyCode == Keys.F3) buttonSearch.PerformClick();
            else if (e.KeyCode == Keys.Enter) buttonEdit.PerformClick();
        }
    }
}
