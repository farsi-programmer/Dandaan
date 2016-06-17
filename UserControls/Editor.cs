using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dandaan.UserControls
{
    public partial class Editor<T> : UserControl where T : class
    {
        public Editor()
        {
            InitializeComponent();
        }

        const string _info_ = "_info_";

        public EditorKind _kind { get; }
        public T _obj { get; protected set; }

        public Editor(PropertyInfo[] propertyInfos, Form form, EditorKind kind = EditorKind.Add,
            T obj = null, Action acceptAct = null, Action cancelAct = null, Action<T> searchAct = null)
        {
            _kind = kind;
            _obj = obj;

            int y = 15, maxX = 0, textBoxWidth = 350, margin = 2, xMargin = 15, tabIndex = 0;
            Label label;
            Color color = Color.Empty;

            foreach (var item in propertyInfos)
            {
                var da = Reflection.GetDandaanColumnAttribute(item);

                //if (item.PropertyType == typeof(string))
                {
                    //

                    var textBox = new Controls.TextBox()
                    {
                        Width = textBoxWidth,
                        Margin = new Padding(0),
                        Padding = new Padding(0),
                        Location = new Point(xMargin, y),
                        ReadOnly = kind == EditorKind.Search ? false : Common.IsMatch(da.Sql, @"[\s]+identity[\s]+"),
                        RightToLeft = RightToLeft.Yes,
                        Name = item.Name,
                        TabIndex = tabIndex++,
                    };

                    textBox.DefaultText = textBox.Text = null != obj ? item.GetValue(obj).ToString() : "";
                    color = textBox.BackColor;

                    textBox.TextChanged += (_, __) =>
                    {
                        if (textBox.Text != textBox.DefaultText)
                        {
                            textBox.BackColor = Color.LightYellow;//LightGoldenrodYellow;//MistyRose;
                            if (kind != EditorKind.Search)
                                (form.AcceptButton as Button).Enabled = true;
                        }
                        else
                        {
                            textBox.BackColor = color;
                            if (kind != EditorKind.Search)
                            {
                                (form.AcceptButton as Button).Enabled = false;
                                foreach (Control A in Controls)
                                    if (A is TextBox && A.Text != (A as Controls.TextBox).DefaultText && !(A as TextBox).ReadOnly)
                                        (form.AcceptButton as Button).Enabled = true;
                            }
                        }

                        if (kind == EditorKind.Search) searchAct(getObj(propertyInfos, kind));
                    };

                    textBox.MouseClick += (_, __) => textBox.SelectAll();

                    if (xMargin + textBox.Width + margin > maxX) maxX = xMargin + textBox.Width + margin;

                    //

                    var labelText = da.Label + ":";

                    label = new Label()
                    {
                        Text = labelText,
                        TextAlign = ContentAlignment.TopRight,
                        //BorderStyle = BorderStyle.FixedSingle,
                        Margin = new Padding(0),
                        Padding = new Padding(0),
                        RightToLeft = RightToLeft.Yes,
                        AutoSize = true,
                    };

                    /*Size s;

                    using (var g = label.CreateGraphics())
                        s = g.MeasureString(labelText, Font).ToSize();

                    label.Size = new Size(s.Width + 5, s.Height);*/

                    label.Location = new Point(0, y);

                    //

                    if (label.Height > textBox.Height) textBox.Location = new Point(textBox.Location.X, textBox.Location.Y
                        + (label.Height - textBox.Height) / 2);
                    else if (textBox.Height > label.Height) label.Location = new Point(label.Location.X, label.Location.Y
                        + (textBox.Height - label.Height) / 2);

                    y += (label.Height > textBox.Height ? label.Height : textBox.Height) + xMargin;

                    //

                    Controls.Add(label);
                    Controls.Add(textBox);
                }
            }

            //

            foreach (Control item in Controls)
                if (item is Label) item.Location = new Point(maxX, item.Location.Y);
                else item.Location = new Point(maxX - item.Width - margin, item.Location.Y);

            foreach (Control item in Controls)
                if (item.Width + item.Location.X > maxX) maxX = item.Width + item.Location.X;

            //

            y += 5;

            var buttonCancel = new Button()
            {
                Text = "انصراف",
                AutoSize = true,
                TabIndex = tabIndex + 1,
            };
            buttonCancel.Location = new Point(maxX - buttonCancel.Width - 5, y);
            buttonCancel.Click += (_, __) => { if (cancelAct == null) form.Close(); else cancelAct(); };
            Controls.Add(buttonCancel);
            form.CancelButton = buttonCancel;

            //

            var buttonAccept = new Button()
            {
                Text = kind == EditorKind.Add ? "اضافه" : kind == EditorKind.Edit ? "ویرایش" : "جستجو",
                AutoSize = true,
                TabIndex = tabIndex++,
                Enabled = false,
                Visible = kind != EditorKind.Search,
            };

            if (kind != EditorKind.Search)
            {
                buttonAccept.Location = new Point(buttonCancel.Location.X - buttonAccept.Width - 8, y);
                buttonAccept.Click += (_, __) =>
                {
                    obj = getObj(propertyInfos, kind);

                //if (blank) MessageBox.Show("لطفا ");
                //else
                {
                        if (kind == EditorKind.Add)
                            SQL.Insert(obj);
                        else if (kind == EditorKind.Edit)
                        {
                            SQL.Update(obj, _obj);
                            _obj = obj;
                        }

                        acceptAct();

                        Controls[_info_].ForeColor = Controls[_info_].ForeColor == Color.Green ? Color.SlateBlue : Color.Green;
                        Controls[_info_].Text = kind == EditorKind.Add ? "اضافه شد" : kind == EditorKind.Edit ? "ویرایش شد" : "جستجو شد";

                        foreach (Control item in Controls)
                            if (item is TextBox)
                                if (kind == EditorKind.Add)
                                    item.Text = "";
                                else if (!(item as TextBox).ReadOnly)
                                {
                                    (item as Controls.TextBox).DefaultText = item.Text;
                                    (item as Controls.TextBox).RaiseTextChanged();
                                //item.GetType().GetMethod(nameof(OnTextChanged), BindingFlags.NonPublic | BindingFlags.Instance)
                                //.Invoke(item, new object[] { new EventArgs() });
                            }
                    }
                };

                Controls.Add(buttonAccept);
                form.AcceptButton = buttonAccept;

                tabIndex++;
            }

            //

            label = new Label()
            {
                TextAlign = ContentAlignment.TopRight,
                Margin = new Padding(0),
                Padding = new Padding(0),
                RightToLeft = RightToLeft.Yes,
                AutoSize = true,
                Name = _info_,
                Location = new Point(10, y + 4),
            };

            Controls.Add(label);

            //

            y += (buttonAccept.Height > label.Height ? buttonAccept.Height : label.Height);

            //

            ClientSize = new Size(maxX + 8, y + 12);
        }

        private T getObj(PropertyInfo[] propertyInfos, EditorKind kind)
        {
            /*T obj;
            var constructor = typeof(T).GetConstructor(new Type[] { typeof(bool) });
            if (constructor != null)
                obj = (T)constructor.Invoke(new object[] { false });
            else
                obj = Activator.CreateInstance<T>();*/
            T obj = Activator.CreateInstance<T>();
            //bool blank = true;

            foreach (var item in propertyInfos)
            {
                item.SetValue(obj, null);

                var p = Controls[item.Name].GetType().GetProperty(nameof(TextBox.ReadOnly));
                var value = Controls[item.Name].Text;

                var t = item.PropertyType;
                var ut = Nullable.GetUnderlyingType(t);
                if (ut != null) t = ut;

                if (p != null)
                {
                    if (!(bool)p.GetValue(Controls[item.Name]))
                    {
                        if (t == typeof(string))
                            item.SetValue(obj, value);
                        else if (t.IsEnum)
                            item.SetValue(obj, Enum.Parse(t, value));
                        else if (t.IsValueType)
                        {
                            //var m = t.GetMethod(nameof(int.Parse), new Type[] { typeof(string) });
                            //var v = m.Invoke(null, new object[] { value });
                            //item.SetValue(obj, v);

                            var m = t.GetMethod(nameof(int.TryParse), new Type[] { typeof(string), t.MakeByRefType() });
                            var ps = new object[] { value, null };
                            var v = m.Invoke(null, ps);
                            if ((bool)v) item.SetValue(obj, ps[1]);
                            //else Controls[item.Name].Text = "";
                            else if (Controls[item.Name].Text != "")
                                Controls[item.Name].BackColor = Color.MistyRose;
                        }
                    }
                    else if (kind == EditorKind.Edit || kind == EditorKind.Search)
                        item.SetValue(obj, item.GetValue(_obj));

                    //if (Controls[item.Name].Text != "") blank = false;
                }
            }

            return obj;
        }

        protected override void OnLoad(EventArgs e)
        {
            foreach (Control item in Controls)
            {
                var p = item.GetType().GetProperty(nameof(TextBox.ReadOnly));
                if (p != null && !(bool)p.GetValue(item))
                { item.Select(); break; }
            }

            base.OnLoad(e);
        }
    }

    public enum EditorKind { Add, Edit, Search };
}
