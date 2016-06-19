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

        int pages => (count / PageSize) + (count % PageSize > 0 ? 1 : 0);
        Editor<T> addForm;
        Timer timer;
        int count = 0;
        bool search = false;
        Control focused;

        protected DandaanAttribute DandaanAttribute;
        protected PropertyInfo[] PropertyInfos;
        protected System.Threading.Thread Thread;
        protected int Page = 1, PageSize = 100;
        protected Func<T, int> CountFunc = SQL.Count;
        protected Action LoadAct;
        protected Func<bool> DeleteFunc;
        protected Action<Action> EditAct;
        protected Control View;
        protected T SearchObj;

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            if (buttonRefresh.Enabled) loadData(Page);
        }

        private void loadData(int page)
        {
            go:
            disable();

            count = CountFunc(SearchObj); // this can be wrong under high load
            if (label1.Text != count.ToString()) label1.Text = count.ToString();
            if (page > pages && page != 1) { page = pages; goto go; }

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
            if (count > 0)
            {
                buttonEdit.Enabled = DandaanAttribute.EnableEdit;
                buttonDelete.Enabled = DandaanAttribute.EnableDelete;
            }
            buttonSearch.Enabled = DandaanAttribute.EnableSearch && !search;

            focused?.Select();
        }

        private void Browser_Load(object sender, EventArgs e)
        {
            View.KeyDown += View_KeyDown;
            View.DoubleClick += View_DoubleClick;
            View.TabIndex = 0;

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
            focused = null;
            foreach (Control item in Controls)
                if (item.Focused) { focused = item; break; }

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
                addForm = new Editor<T>(DandaanAttribute.Label + " - اضافه");
                var editor = new UserControls.Editor<T>(PropertyInfos, addForm, acceptAct: acceptAct);
                addForm.setEditor(editor);
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

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            var h = View.Height;
            var l = View.Location;

            var panel = new Panel()
            {
                Width = ClientSize.Width,
                AutoScroll = true,
            };

            var editor = new UserControls.Editor<T>(PropertyInfos, this, UserControls.EditorKind.Search,
                searchAct: (T searchObj) =>
                {
                    SearchObj = searchObj;
                    buttonRefresh.PerformClick();
                },
                cancelAct: () =>
                {
                    View.Height = h;
                    View.Location = l;
                    search = false;
                    buttonSearch.Enabled = true;
                    foreach (Control item in panel.Controls)
                    {
                        foreach (Control A in item.Controls) A.Dispose();
                        item.Dispose();
                    }
                    Controls.Remove(panel);
                    panel.Dispose();
                    
                    SearchObj = null;
                    buttonRefresh.PerformClick();
                });

            panel.Height = editor.Height;
            editor.Location = new Point(panel.Width - editor.Width - 18, 0);
            var half = View.Height / 2;
            if (panel.Height > half) panel.Height = half;

            panel.Controls.Add(editor);

            View.Height -= panel.Height;
            View.Location = new Point(View.Location.X, View.Location.Y + panel.Height);

            search = true;
            buttonSearch.Enabled = false;
            Controls.Add(panel);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            buttonRefresh.PerformClick();
        }

        private void View_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete) buttonDelete.PerformClick();
            else if (e.KeyCode == Keys.Insert) buttonAdd.PerformClick();
            else if (e.KeyCode == Keys.F3) buttonSearch.PerformClick();
            else if (e.KeyCode == Keys.Enter) buttonEdit.PerformClick();
        }
    }
}
