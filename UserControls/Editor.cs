// http://offtopic.blog.ir/

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

            int y = 15, maxX = 0, textBoxWidth = /*350*/400, margin = 2, xMargin = 15, tabIndex = 0;
            Label label;
            Color color = Color.Empty;

            Action<Control, PropertyInfo, DandaanColumnAttribute> act = (control, item, da) =>
            {
                control.Width = textBoxWidth;
                control.Margin = new Padding(0);
                control.Padding = new Padding(0);
                control.Location = new Point(xMargin, y);
                control.RightToLeft = RightToLeft.Yes;
                control.Name = item.Name;
                control.TabIndex = tabIndex++;

                //

                if (control is TextBox)
                {
                    if (da.Multiline)
                    {
                        (control as TextBox).Multiline = true;
                        control.Height = 80;//55;
                        (control as TextBox).ScrollBars = ScrollBars.Both;
                        (control as TextBox).AcceptsReturn = true;
                    }

                    if (kind != EditorKind.Search)
                    {
                        if (Common.IsMatch(da.Sql, @"[\s]+identity[\s]+"))
                            (control as TextBox).ReadOnly = true;
                        else if (kind == EditorKind.Edit && Reflection.GetColumnAttribute(item).IsPrimaryKey)
                            (control as TextBox).ReadOnly = true;
                    }
                }
                else if (control is ComboBox)
                {
                    //(control as ComboBox).DropDownStyle = ComboBoxStyle.DropDownList;
                    (control as ComboBox).AutoCompleteSource = AutoCompleteSource.ListItems;
                    (control as ComboBox).AutoCompleteMode = AutoCompleteMode.Append;

                    if (kind == EditorKind.Edit && Reflection.GetColumnAttribute(item).IsPrimaryKey)
                        control.Enabled = false;
                }

                //

                if (null != obj)
                {
                    var value = item.GetValue(obj);
                    control.Text = value != null ? value.ToString() : "";
                }

                if (control is TextBox)
                    (control as Controls.TextBox).DefaultText = control.Text;
                else if (control is ComboBox)
                {
                    if (Nullable.GetUnderlyingType(item.PropertyType) != null)
                    {
                        if (kind == EditorKind.Search)
                        {
                            if (SQL.isForeignKey(da.Sql))
                                (control as Controls.ComboBox).Items.Add(new ComboboxItem { Text = ""/*, Value = 0*/ });
                            else (control as Controls.ComboBox).Items.Add("");
                        }
                        else
                        {
                            if (!SQL.isNotNull(da.Sql))
                            {
                                if (SQL.isForeignKey(da.Sql))
                                    (control as Controls.ComboBox).Items.Add(new ComboboxItem { Text = ""/*, Value = 0*/ });
                                else (control as Controls.ComboBox).Items.Add("");
                            }
                        }
                    }

                    if (SQL.isForeignKey(da.Sql))
                    {
                        var type = Type.GetType($"{nameof(Dandaan)}.{nameof(Tables)}.{SQL.getForeignTable(da.Sql)}");

                        var result = typeof(SQL).GetMethod(nameof(SQL.SelectAll)).MakeGenericMethod(type)
                        .Invoke(null, null);

                        foreach (var B in (System.Collections.IEnumerable)result)
                        {
                            var text = type.GetProperty(da.ForeignTableDisplayColumn).GetValue(B).ToString();
                            var value = type.GetProperty(SQL.getForeignColumn(da.Sql)).GetValue(B);

                            var i = (control as ComboBox).Items.Add(new ComboboxItem
                            {
                                Value = value,
                                Text = text
                            });

                            if (control.Text == value.ToString())
                            {
                                control.Text = text;
                                (control as ComboBox).SelectedIndex = i;
                            }
                        }
                    }
                    else
                    {
                        if (item.PropertyType.IsEnum)
                            (control as Controls.ComboBox).Items.AddRange(Enum.GetNames(item.PropertyType));
                        else
                            (control as Controls.ComboBox).Items.AddRange(Enum.GetNames(Nullable.GetUnderlyingType(item.PropertyType)));
                    }

                    if (control.Text == "" && (control as ComboBox).Items.Count > 0)
                         (control as ComboBox).SelectedIndex = 0;

                    (control as Controls.ComboBox).DefaultText = control.Text;

                    (control as ComboBox)./*Leave*/LostFocus += (_, __) =>
                    {
                        if (!form.Disposing)
                        {
                            var contains = (control as ComboBox).Items.Contains(control.Text);

                            if (!contains && (control as ComboBox).Items.Count > 0 && (control as ComboBox).Items[0] is ComboboxItem)
                                foreach (ComboboxItem B in (control as ComboBox).Items)
                                    if (B.Text == control.Text) { contains = true; break; }

                            if (control.Text != (control as Controls.ComboBox).DefaultText && !contains)
                                control.Text = (control as Controls.ComboBox).DefaultText;
                        }
                    };

                    (control as ComboBox).PreviewKeyDown += (_, __) =>
                    {
                        if (__.KeyCode == Keys.Enter) (control as Controls.ComboBox).RaiseLostFocus();
                    };
                }

                //

                color = control.BackColor;

                var textChanged = new EventHandler((_, __) =>
                {
                    if ((control is TextBox && control.Text != (control as Controls.TextBox).DefaultText)
                    || (control is ComboBox && control.Text != (control as Controls.ComboBox).DefaultText))
                    {
                        control.BackColor = Color.LightYellow;//LightGoldenrodYellow;//MistyRose;
                        if (kind != EditorKind.Search)
                            (form.AcceptButton as Button).Enabled = true;
                    }
                    else
                    {
                        control.BackColor = color;
                        if (kind != EditorKind.Search)
                        {
                            (form.AcceptButton as Button).Enabled = false;

                            foreach (Control A in Controls)
                                if ((A is TextBox && A.Text != (A as Controls.TextBox).DefaultText && !(A as TextBox).ReadOnly)
                                || (A is ComboBox && A.Text != (A as Controls.ComboBox).DefaultText))
                                    (form.AcceptButton as Button).Enabled = true;
                        }
                    }

                    if (kind == EditorKind.Search)
                    {
                        var contains = false;

                        if (control is ComboBox)
                        {
                            contains = (control as ComboBox).Items.Contains(control.Text);

                            if (!contains && (control as ComboBox).Items.Count > 0 && (control as ComboBox).Items[0] is ComboboxItem)
                                foreach (ComboboxItem B in (control as ComboBox).Items)
                                    if (B.Text == control.Text) { contains = true; break; }
                        }

                        if (!(control is ComboBox && !contains)) searchAct(getObj(propertyInfos, kind));
                    }
                });

                //

                if (control is TextBox)
                    (control as TextBox).TextChanged += textChanged;
                else if (control is ComboBox)
                    (control as ComboBox).TextChanged += textChanged;

                //

                if (xMargin + control.Width + margin > maxX) maxX = xMargin + control.Width + margin;
            };

            foreach (var item in propertyInfos)
            {
                var ut = Nullable.GetUnderlyingType(item.PropertyType);

                var da = Reflection.GetDandaanColumnAttribute(item);

                Control control = null;

                //

                if (item.PropertyType.IsEnum || (ut != null && ut.IsEnum)
                    || SQL.isForeignKey(da.Sql))
                {
                    control = new Controls.ComboBox();
                    act(control, item, da);
                }
                else
                {
                    control = new Controls.TextBox();
                    act(control, item, da);

                    control.MouseClick += (_, __) => (control as Controls.TextBox).SelectAll();
                }

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

                if (label.Height > control.Height) control.Location = new Point(control.Location.X, control.Location.Y
                    + (label.Height - control.Height) / 2);
                else if (control.Height > label.Height) label.Location = new Point(label.Location.X, label.Location.Y
                    + (control.Height - label.Height) / 2);

                y += (label.Height > control.Height ? label.Height : control.Height) + xMargin;

                //

                Controls.Add(label);
                Controls.Add(control);
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
                            {
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
                            else if (item is ComboBox)
                            {
                                if (kind == EditorKind.Add)
                                    item.Text = (item as Controls.ComboBox).DefaultText;
                                else
                                {
                                    (item as Controls.ComboBox).DefaultText = item.Text;
                                    item.Text = item.Text + " ";
                                    item.Text = (item as Controls.ComboBox).DefaultText;
                                    // not working (item as Controls.ComboBox).RaiseTextChanged();
                                }
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

                var t = item.PropertyType;
                var ut = Nullable.GetUnderlyingType(t);
                if (ut != null) t = ut;

                var p = Controls[item.Name].GetType().GetProperty(nameof(TextBox.ReadOnly));

                if (Controls[item.Name] is ComboBox || p != null)
                {
                    var value = Controls[item.Name].Text;

                    if (Controls[item.Name] is ComboBox && (Controls[item.Name] as ComboBox).Items.Count > 0
                        && (Controls[item.Name] as ComboBox).Items[0] is ComboboxItem
                        && (Controls[item.Name] as ComboBox).SelectedItem != null
                        && ((Controls[item.Name] as ComboBox).SelectedItem as ComboboxItem).Value != null)
                        value = ((Controls[item.Name] as ComboBox).SelectedItem as ComboboxItem).Value.ToString();

                    if (Controls[item.Name] is ComboBox || !(bool)p.GetValue(Controls[item.Name]))
                    {
                        if (t == typeof(string))
                            item.SetValue(obj, value);
                        else if (t.IsEnum)
                        {
                            if (value as string != "") item.SetValue(obj, Enum.Parse(t, value as string));
                        }
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

    public class ComboboxItem
    {
        public string Text { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }

    public enum EditorKind { Add, Edit, Search };
}
