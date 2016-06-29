using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dandaan.Forms
{
    public partial class Browser<T, C> : Form where T : class where C : Control, new()
    {
        public Browser()
        {
            InitializeComponent();

            addView();

            FormClosing += Browser_FormClosing;

            DandaanAttribute = Reflection.GetDandaanAttribute(typeof(T));

            PropertyInfos = typeof(T).GetProperties();
        }

        void addView()
        {
            View = new C();

            View.Location = new Point(12, 12);
            View.RightToLeft = RightToLeft.Yes;
            View.Size = new Size(760, 495);
            View.TabIndex = 1;

            if (View is DataGridView)
            {
                (View as DataGridView).ColumnHeadersHeightSizeMode =
                    DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            }
            else if (View is ListView)
            {
                (View as ListView).FullRowSelect = true;
                (View as ListView).MultiSelect = false;
                (View as ListView).RightToLeftLayout = true;
                (View as ListView).UseCompatibleStateImageBehavior = false;
                (View as ListView).View = System.Windows.Forms.View.Details;
            }
            else if (View is TextBoxBase) // including RichTextBox
            {
                View.RightToLeft = RightToLeft.No;
                (View as TextBoxBase).ReadOnly = true;
                (View as TextBoxBase).Multiline = true;
            }
            else if (View is TextBox)
            {
                (View as TextBox).ScrollBars = ScrollBars.Both;
            }
            else if (View is ListBox) // including CheckedListBox
            {
                (View as ListBox).ItemHeight = 24;
                (View as ListBox).FormattingEnabled = true;
            }

            SuspendLayout();
            Controls.Add(View);
            ResumeLayout(false);
            PerformLayout();
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
        DandaanAttribute DandaanAttribute;
        PropertyInfo[] PropertyInfos;
        System.Threading.Thread Thread;
        int Page = 1, PageSize = 100;
        Func<T, int> CountFunc = SQL.Count;
        C View;
        T SearchObj;

        public Action AfterEdit = () => { };
        public Action AfterAdd = () => { };
        public Action AfterDelete = () => { };

        public Func<T, bool> BeforeEdit = (_) => true;
        public Func<T, bool> BeforeAdd = (_) => true;
        public Func<T, bool> BeforeDelete = (_) => true;

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

            if (View is ListBox) listBoxLoad();
            else if (View is TextBoxBase) textBoxBaseLoad();
            else if (View is ListView) listViewLoad();
            else if (View is DataGridView) dataGridViewLoad();
        }

        void listBoxLoad()
        {
            Thread = Common.Thread(() =>
            {
                var ListBox = View as ListBox;

                var objs = SQL.SelectWithWhere(Page, PageSize, SearchObj, true).Select((row) =>
                {
                    var sb = new StringBuilder();
                    foreach (var item in PropertyInfos)
                    {
                        var obj = item.GetValue(row);

                        if (obj is string) obj = Regex.Replace(obj as string, "[\r\n]+", " ");

                        sb.Append(obj + "\t");
                    }
                    return sb.ToString();
                }).ToArray();

                Invoke(() =>
                {
                    if (objs == null || objs.Length == 0)
                    {
                        ListBox.Items.Clear();
                        ListBox.Items.Add("رکوردی وجود ندارد.");
                    }
                    else
                    {
                        if (ListBox.Items.Count == 0) ListBox.Items.AddRange(objs);
                        else
                        {
                            for (int i = 0; i < ListBox.Items.Count; i++)
                                if (objs.Length > i)
                                {
                                    if (ListBox.Items[i].ToString() != objs[i].ToString())
                                    {
                                        if (ListBox.Visible) ListBox.Hide();

                                        ListBox.Items[i] = objs[i];

                                        if (ListBox.SelectedIndex == i) ListBox.ClearSelected();

                                        if (ListBox is CheckedListBox && (ListBox as CheckedListBox).GetItemChecked(i))
                                            (ListBox as CheckedListBox).SetItemChecked(i, false);
                                    }
                                }
                                else
                                {
                                    if (ListBox.Visible) ListBox.Hide();
                                    while (ListBox.Items.Count > i) ListBox.Items.RemoveAt(i);
                                    break;
                                }

                            if (ListBox.Items.Count < objs.Length)
                            {
                                if (ListBox.Visible) ListBox.Hide();
                                for (int i = ListBox.Items.Count; i < objs.Length; i++)
                                    ListBox.Items.Add(objs[i]);
                            }

                            /*if (ListBox.SelectedIndex < 0)
                            {
                                ListBox.SelectedIndex = 0;
                                ListBox.ClearSelected();
                            }*/

                            if (!ListBox.Visible) ListBox.Show();
                        }
                    }

                    enable();
                });
            });

            Thread.Start();

            // testing
            //browserMenu1.CountFunc = () => 1000;
            //var x = new List<object>(1000);
            //for (int i = 0; i < 1000; i++) x.Add(i);
            //ObjsFunc = () => { return x.Skip((browserMenu1.Page - 1) * browserMenu1.PageSize).Take(browserMenu1.PageSize).Select((k) => (object)(((int)k) + DateTime.Now.Second)).ToArray(); };
        }

        void textBoxBaseLoad()
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
            Thread = Common.Thread(() =>
            {
                var TextBox = View as TextBoxBase;

                var sb = new StringBuilder();
                RichTextBox rtb = null;

                if (TextBox is RichTextBox)
                {
                    rtb = new RichTextBox() { Location = new Point(Width, Height) };
                    rtb.Hide();
                    Invoke(() => Controls.Add(rtb));
                }

                var lines = SQL.SelectWithWhere(Page, PageSize, SearchObj, true).Select((row) =>
                {
                    var A = new StringBuilder();
                    foreach (var item in PropertyInfos)
                    {
                        var obj = item.GetValue(row);

                        // replacing new lines is specially necessary if we want to get the id for edit or delete
                        if (obj is string) obj = Regex.Replace(obj as string, "[\r\n]+", " ");

                        A.Append(obj + "\t");
                    }
                    return A.ToString();
                });

                bool odd = false;
                foreach (var item in lines)
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

                    enable();
                });
            });

            Thread.Start();
#endif

            // testing
            //browserMenu1.CountFunc = () => 1000;
            //var x = new List<object>(1000);
            //for (int i = 0; i < 1000; i++) x.Add(i);
            //LinesFunc = () => { return x.Skip((browserMenu1.Page - 1) * browserMenu1.PageSize).Take(browserMenu1.PageSize).Select((k) => (object)(((int)k) + DateTime.Now.Second)).ToArray(); };

            // this is if we want to be able to undo, but we don't, we have it in db
            //str = Regex.Replace(log.Message, Regex.Escape(@"\r\n"), @"\\r\\n");
            //sb.Append($"{log.Id}\t{log.DateTime}\t{Regex.Replace(str, "\r\n", @"\r\n")}\r\n");
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

        const string NoRecords = "رکوردی وجود ندارد.";
        const string Blank = "";
        List<T> original, _original;

        void listViewLoad()
        {
            var listView1 = View as ListView;

            // testing
            //browserMenu1.CountFunc = () => 1000;
            //var x = new List<ListViewItem>(1000);
            //for (int i = 0; i < 1000; i++) x.Add(new ListViewItem(i.ToString()));
            //ItemsFunc = () => { return x.Skip((browserMenu1.Page - 1) * browserMenu1.PageSize).Take(browserMenu1.PageSize).Select((k) => { Invoke(new Action(() => k.Text = k.Text + DateTime.Now.Second)); /*new ListViewItem(k.Text + DateTime.Now.Second)*/ return k; }).ToArray(); };
            //listView1.Columns.Add("test", 200);

            Thread = Common.Thread(() =>
            {
                _original = new List<T>();
                bool odd = true;
                var items = SQL.SelectWithWhere(Page, PageSize, SearchObj, true).Select((row) =>
                {
                    _original.Add(row);
                    odd = !odd;
                    return new ListViewItem(PropertyInfos.Select(item =>
                    {
                        var da = Reflection.GetDandaanColumnAttribute(item);

                        var value = item.GetValue(row);

                        if (value != null && SQL.IsForeignKey(da.Sql))
                        {
                            var type = Type.GetType($"{nameof(Dandaan)}.{nameof(Tables)}.{SQL.GetForeignTable(da.Sql)}");

                            var obj = Activator.CreateInstance(type);//, new object[] { false});

                            foreach (var A in type.GetProperties())
                                if (A.GetValue(obj) != null && Nullable.GetUnderlyingType(A.PropertyType) != null)
                                    A.SetValue(obj, null);

                            var ps = type.GetProperties();

                            ps.Where(p => p.Name == SQL.GetForeignColumn(da.Sql)).First()
                            .SetValue(obj, value);

                            var result = typeof(SQL).GetMethod(nameof(SQL.SelectFirstWithWhere)).MakeGenericMethod(type)
                            .Invoke(null, new object[] { obj, false });

                            foreach (var B in ps) if (B.Name != SQL.GetForeignColumn(da.Sql)
                                && !SQL.IsForeignKey(Reflection.GetDandaanColumnAttribute(B).Sql))
                                    value += "  " + B.GetValue(result);
                        }

                        return value != null ? value.ToString() : "";
                    }).ToArray())
                    { BackColor = odd ? Color.LightCyan : Color.FromArgb(0xcf, 0xff, 0xcf) };
                }).ToArray();

                Invoke(() =>
                {
                    original = _original;

                    if (items == null || items.Length == 0)
                    {
                        listView1.Columns.Clear();
                        listView1.Columns.Add(Blank);

                        listView1.Items.Clear();
                        listView1.Items.Add(NoRecords);

                        listView1.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                    }
                    else
                    {
                        if (listView1.Items.Count == 0 ||
                        (listView1.Items.Count == 1 && listView1.Columns.Count == 1 &&
                        listView1.Columns[0].Text == Blank && listView1.Items[0].Text == NoRecords))
                        {
                            listView1.Columns.Clear();
                            listView1.Items.Clear();

                            //using (var g = CreateGraphics())
                            {
                                int sum = 0;
                                //SizeF size;
                                foreach (var item in PropertyInfos)
                                {
                                    var label = Reflection.GetDandaanAttribute(item).Label;

                                    //size = g.MeasureString(label, Font);

                                    //listViewBrowser1.listView1.Columns.Add(label).Width = (int)size.Width + 10;
                                    var c = new ColumnHeader() { Text = label };
                                    c.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
                                    listView1.Columns.Add(c);

                                    //sum += (int)size.Width + 10;
                                    c.Width += 30;
                                    sum += c.Width + 5;
                                }

                                if (listView1.Width > sum && (listView1.Width - sum) / PropertyInfos.Length > 1)
                                {
                                    var x = (listView1.Width - sum) / PropertyInfos.Length;

                                    for (int i = 0; i < listView1.Columns.Count; i++)
                                        listView1.Columns[i].Width = listView1.Columns[i].Width
                                            + ((PropertyInfos[i].PropertyType == typeof(int)) ? 0 : x--);
                                }
                            }

                            listView1.Items.AddRange(items);
                        }
                        else
                        {
                            for (int i = 0; i < listView1.Items.Count; i++)
                                if (items.Length > i)
                                {
                                    //if (listView1.Items[i].Text != items[i].Text)
                                    //listView1.Items[i] = items[i];

                                    for (int j = 0; j < listView1.Items[i].SubItems.Count; j++)
                                        if (listView1.Items[i].SubItems[j].Text != items[i].SubItems[j].Text)
                                        {
                                            // we can create a new ListViewItem for each round or
                                            // we can manipulate the existing one

                                            listView1.Items[i] = items[i];
                                            break;

                                            //listView1.Items[i].SubItems[j] = items[i].SubItems[j];
                                        }

                                    if (listView1.Items[i].BackColor != items[i].BackColor)
                                        listView1.Items[i].BackColor = items[i].BackColor;
                                }
                                else
                                {
                                    if (listView1.Visible) listView1.Hide();
                                    while (listView1.Items.Count > i) listView1.Items.RemoveAt(i);
                                    break;
                                }

                            for (int i = listView1.Items.Count; i < items.Length; i++)
                                listView1.Items.Add(items[i]);

                            /*if (listView1.SelectedIndices.Count == 0)
                                listView1.EnsureVisible(0);*/

                            if (!listView1.Visible) listView1.Show();
                        }
                    }

                    enable();
                });
            });

            Thread.Start();
        }

        T GetObj()
        {
            if (View is ListView)
            {
                var listView1 = View as ListView;

                if (listView1.SelectedItems.Count == 0) return null;

                var obj = Activator.CreateInstance<T>();

                for (int i = 0; i < PropertyInfos.Length; i++)
                {
                    /*var p = PropertyInfos[i];
                    var value = listView1.SelectedItems[0].SubItems[i].Text;

                    var t = p.PropertyType;
                    var ut = Nullable.GetUnderlyingType(t);
                    if (ut != null) t = ut;

                    if (t == typeof(string))
                        p.SetValue(obj, value);
                    else if (t.IsEnum)
                        p.SetValue(obj, Enum.Parse(t, value));
                    else if (t.IsValueType)
                    {
                        if (SQL.isForeignKey(Reflection.GetDandaanColumnAttribute(p).Sql))
                            p.SetValue(obj, p.GetValue(original[listView1.SelectedItems[0].Index]));
                        else
                            p.SetValue(obj, t.GetMethod(nameof(int.Parse), new Type[] { typeof(string) })
                                .Invoke(null, new object[] { value }));
                    }*/
                    obj = original[listView1.SelectedItems[0].Index];
                }

                return obj;
            }

            return null;
        }

        bool DeleteFunc()
        {
            if (View is ListView)
            {
                var listView1 = View as ListView;

                if (listView1.SelectedIndices.Count < 1) MessageBox.Show("لطفا یک رکورد را برای حذف کردن انتخاب کنید.‏", Program.Title);
                else if (MessageBox.Show("آیا مطمئن هستید که میخواهید این رکورد را حذف کنید؟"
                     + "\r\n" + listView1.SelectedItems[0].Text,
                    Program.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    var obj = GetObj();

                    if (BeforeDelete(obj))
                    {
                        SQL.Delete(obj);

                        return true;
                    }
                }
            }

            return false;
        }

        void EditAct(Action acceptAct)
        {
            if (View is ListView)
            {
                var listView1 = View as ListView;

                if (listView1.SelectedIndices.Count < 1) MessageBox.Show("لطفا یک رکورد را برای ویرایش کردن انتخاب کنید.‏", Program.Title);
                else
                {
                    var obj = GetObj();

                    if (BeforeEdit(obj))
                    {
                        Func<T, bool> equals = (_obj) =>
                        {
                            foreach (var item in PropertyInfos)
                                if (!item.GetValue(_obj).Equals(item.GetValue(obj)))
                                    return false;

                            return true;
                        };

                        foreach (Form item in Application.OpenForms)
                            foreach (var A in item.Controls)
                                if (A is UserControls.Editor<T>
                                && (A as UserControls.Editor<T>)._kind == UserControls.EditorKind.Edit
                                && equals((A as UserControls.Editor<T>)._obj))
                                {
                                    var f = item;
                                    ShowForm(ref f, false);

                                    return;
                                }

                        var editForm = new Editor<T>(DandaanAttribute.Label + " - ویرایش");
                        var editor = new UserControls.Editor<T>(PropertyInfos, editForm, UserControls.EditorKind.Edit, obj, acceptAct);
                        editForm.setEditor(editor);

                        ShowForm(ref editForm, false);
                    }
                }
            }
        }

        void dataGridViewLoad()
        {
            var dataGridView1 = View as DataGridView;

            // testing
            CountFunc = (s) => 1000;
            var x = new List<DataGridViewRow>(1000);
            for (int i = 0; i < 1000; i++) { var r = new DataGridViewRow(); r.Cells.Add(new DataGridViewTextBoxCell() { Value = i }); x.Add(r); }
            Func<DataGridViewRow[]> ArrayFunc = () => { return x.Skip((Page - 1) * PageSize).Take(PageSize).Select((k) => { Invoke(new Action(() => { k.Cells[0].Value = (int)k.Cells[0].Value + DateTime.Now.Second; })); return k; }).ToArray(); };
            dataGridView1.Columns.Add("test", "test");
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            //
            // datagridview is complicated, this is incomplete!
            //

            Thread = Common.Thread(() =>
            {
                var objs = ArrayFunc();

                Invoke(() =>
                {
                    if (objs == null || objs.Length == 0)
                    {
                        dataGridView1.Columns.Clear();
                        dataGridView1.Columns.Add("", "");
                        dataGridView1.Rows.Clear();
                        dataGridView1.Rows.Add("رکوردی وجود ندارد.");
                        dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    }
                    else
                    {
                        if (dataGridView1.Rows.Count == 0 || (dataGridView1.Rows.Count == 1
                        && dataGridView1.Rows[0].Cells.Count == 1
                        && dataGridView1.Rows[0].Cells[0].Value == null))
                            dataGridView1.Rows.AddRange(objs);
                        else
                        {
                            /*for (int i = 0; i < dataGridView1.Rows.Count; i++)
                                if (objs.Length > i)
                                {
                                    //foreach (var item in dataGridView1.Rows[i].Cells)
                                    {
                                        if (dataGridView1.Rows[i].Cells[0].Value != objs[i].Cells[0].Value)
                                            dataGridView1.Rows[i].Cells[0].Value = objs[i].Cells[0].Value;
                                    }
                                }
                                else dataGridView1.Rows.RemoveAt(i);

                            for (int i = dataGridView1.Rows.Count; i < objs.Length; i++)
                                dataGridView1.Rows.Add(objs[i]);*/
                        }
                    }

                    enable();
                });
            });

            Thread.Start();
        }


        void enable()
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

            timer = new Timer() { Interval = /*3000*/10000, Enabled = true };
            timer.Tick += Timer_Tick;
        }

        private void View_DoubleClick(object sender, EventArgs e)
        {
            if (selectOnDoubleClick) Close();
            else buttonEdit.PerformClick();
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
            foreach (Control item in Controls) if (item.Focused) { focused = item; break; }

            buttonRefresh.Enabled = buttonFirst.Enabled = buttonLast.Enabled = buttonNext.Enabled
            = buttonPrevious.Enabled = textBox2.Enabled = buttonDelete.Enabled = buttonEdit.Enabled = false;
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            bool isChecked = checkBox1.Checked;
            if (isChecked) checkBox1.Checked = false;

            var success = DeleteFunc();

            checkBox1.Checked = isChecked;
            if (success) { buttonRefresh.PerformClick(); AfterDelete(); }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (addForm == null || addForm.IsDisposed)
            {
                addForm = new Editor<T>(DandaanAttribute.Label + " - اضافه");
                var editor = new UserControls.Editor<T>(PropertyInfos, addForm, acceptAct: () => acceptAct(AfterAdd), beforeAdd: BeforeAdd);
                addForm.setEditor(editor);
            }

            ShowForm(ref addForm, false);
        }

        Action<Action> acceptAct => (act) =>
        {
            if (Visible) buttonRefresh.PerformClick();
            else foreach (var item in Application.OpenForms)
                    if (item is Browser<T, C>)
                        (item as Browser<T, C>).buttonRefresh.PerformClick();

            act?.Invoke();
        };

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            bool isChecked = checkBox1.Checked;
            if (isChecked) checkBox1.Checked = false;

            EditAct(() => acceptAct(AfterEdit));

            checkBox1.Checked = isChecked;
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            var h = View.Height;
            var l = View.Location;

            var panel = new Panel()
            {
                Width = ClientSize.Width,
                //Anchor = AnchorStyles.None,
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
            else if (e.KeyCode == Keys.Enter)
            {
                if (selectOnDoubleClick) Close();
                else buttonEdit.PerformClick();
            }
            else if (e.KeyCode == Keys.F5) buttonRefresh.PerformClick();
        }

        bool selectOnDoubleClick = false;
        bool cancelSelection = false;

        private void Browser_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && selectOnDoubleClick) Close();
        }

        public T ShowAndReturnSelection()//T selection = null)
        {
            selectOnDoubleClick = true;
            KeyPreview = true;

            buttonEdit.Visible = buttonDelete.Visible = buttonAdd.Visible = false;

            var buttonSelect = new Button()
            {
                Text = "انتخاب",
                Height = buttonEdit.Height,
                Location = new Point(buttonEdit.Location.X - 21, buttonEdit.Location.Y)
            };
            buttonSelect.Click += (_, __) => { Close(); };
            Controls.Add(buttonSelect);

            var buttonCancel = new Button()
            {
                Text = "انصراف",
                Height = buttonEdit.Height,
                Location = new Point(buttonSelect.Location.X - buttonSelect.Width - 3, buttonSelect.Location.Y)
            };
            buttonCancel.Click += (_, __) => { cancelSelection = true; Close(); };
            Controls.Add(buttonCancel);

            ShowDialog();

            return cancelSelection ? null : GetObj();
        }
    }
}
