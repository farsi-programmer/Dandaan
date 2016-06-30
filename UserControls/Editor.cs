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

            //Anchor = AnchorStyles.None;
        }

        const string _info_ = "_info_";

        public EditorKind _kind { get; }
        public T _obj { get; protected set; }

        public Editor(PropertyInfo[] propertyInfos, Form form, EditorKind kind = EditorKind.Add,
            T obj = null, Action acceptAct = null, Action cancelAct = null, Action<T> searchAct = null,
            Func<T, bool> beforeAdd = null)
            : this()
        {
            _kind = kind;
            _obj = obj;
            var defaultObj = Activator.CreateInstance(typeof(T));

            int y = 15, maxX = 0, width = /*350*/450, margin = 2, xMargin = 15, tabIndex = 0, yMargin = 8;
            Label label;
            Color color = Color.Empty;

            Action<Control, PropertyInfo, DandaanColumnAttribute> act = (control, item, da) =>
            {
                Action<Control> common = (c) =>
                {
                    c.Width = width;
                    c.Margin = new Padding(0);
                    c.Padding = new Padding(0);
                    //c.Anchor = AnchorStyles.None;
                    c.Location = new Point(xMargin, y);
                    if (!(c is UserControl)) c.RightToLeft = RightToLeft.Yes;
                    c.Name = item.Name;
                    c.TabIndex = tabIndex++;

                    if (null != obj)
                    {
                        var value = item.GetValue(obj);
                        c.Text = value != null ? value.ToString() : "";
                    }
                };

                common(control);

                //

                if (control is TextBox)
                {
                    if (da.Multiline)
                    {
                        (control as TextBox).Multiline = true;
                        control.Height = 105;//80;
                        (control as TextBox).ScrollBars = ScrollBars.Both;
                        (control as TextBox).AcceptsReturn = true;
                    }

                    if (kind != EditorKind.Search)
                    {
                        if (SQL.IsIdentity(da.Sql))
                            (control as TextBox).ReadOnly = true;
                        else if (kind == EditorKind.Edit && Reflection.GetColumnAttribute(item).IsPrimaryKey)
                            (control as TextBox).ReadOnly = true;
                    }

                    (control as Controls.TextBox).DefaultText = control.Text;
                }
                else if (control is ComboBox)
                {
                    //(control as ComboBox).DropDownStyle = ComboBoxStyle.DropDownList;
                    (control as ComboBox).AutoCompleteSource = AutoCompleteSource.ListItems;
                    (control as ComboBox).AutoCompleteMode = AutoCompleteMode.Append;

                    if (kind == EditorKind.Edit && Reflection.GetColumnAttribute(item).IsPrimaryKey)
                        control.Enabled = false;

                    if (Nullable.GetUnderlyingType(item.PropertyType) != null)
                    {
                        if (kind == EditorKind.Search)
                        {
                            (control as Controls.ComboBox).Items.Add("");
                        }
                        else
                        {
                            if (!SQL.IsNotNull(da.Sql))
                            {
                                (control as Controls.ComboBox).Items.Add("");
                            }
                        }
                    }

                    if (item.PropertyType.IsEnum)
                        (control as Controls.ComboBox).Items.AddRange(Enum.GetNames(item.PropertyType));
                    else
                        (control as Controls.ComboBox).Items.AddRange(Enum.GetNames(Nullable.GetUnderlyingType(item.PropertyType)));

                    if (control.Text == "" && (control as ComboBox).Items.Count > 0)
                    {
                        int i = 0;

                        var v = item.GetValue(defaultObj);
                        if (v != null) i = (int)v;

                        (control as ComboBox).SelectedIndex = i;
                    }

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
                else if (control is UserControl)
                {
                    var button = new Controls.ButtonEdit();
                    common(button);
                    button.Location = new Point(0, 0);
                    button.Name = button.Name + nameof(Dandaan.Controls.ButtonEdit);
                    control.Controls.Add(button);

                    button.TextAlign = ContentAlignment.MiddleLeft;
                    button.Height = 32;
                    control.Height = 32;

                    if (kind == EditorKind.Edit && Reflection.GetColumnAttribute(item).IsPrimaryKey)
                        control.Enabled = false;

                    if (Nullable.GetUnderlyingType(item.PropertyType) != null)
                    {
                        Action addClear = () =>
                        {
                            var clear = new Button();
                            common(clear);
                            clear.Location = new Point(0, 0);
                            clear.Name = clear.Name + nameof(Button);
                            control.Controls.Add(clear);

                            clear.TextAlign = ContentAlignment.MiddleLeft;
                            clear.Text = "پاک";
                            clear.Width = 40;
                            clear.Height = 32;
                            button.Width = button.Width - clear.Width - 3;
                            button.Location = new Point(button.Location.X + clear.Width + 3, button.Location.Y);

                            clear.Click += (_, __) => { button.Text = ""; button.Obj = null; };
                        };

                        if (kind == EditorKind.Search)
                        {
                            addClear();
                        }
                        else
                        {
                            if (!SQL.IsNotNull(da.Sql))
                            {
                                addClear();
                            }
                        }
                    }

                    var name = $"{nameof(Dandaan)}.{nameof(Tables)}.{SQL.GetForeignTable(da.Sql)}";
                    var type = Type.GetType(name);

                    if (type == null)
                    {
                        var assembly = Reflection.LoadAssembly(Program.DataDirectory + "\\" + name + ".dll");
                        type = assembly.GetType(name);
                    }

                    if (control.Text != "")
                    {
                        var instance = Activator.CreateInstance(type);

                        foreach (var A in type.GetProperties())
                            if (A.GetValue(instance) != null && Nullable.GetUnderlyingType(A.PropertyType) != null)
                                A.SetValue(instance, null);

                        var ps = type.GetProperties();

                        var property = ps.Where(p => p.Name == SQL.GetForeignColumn(da.Sql)).First();

                        property.SetValue(instance, /*control.Text*/item.GetValue(obj));

                        var result = typeof(SQL).GetMethod(nameof(SQL.SelectFirstWithWhere)).MakeGenericMethod(type)
                        .Invoke(null, new object[] { instance, false });

                        button.Obj = result;

                        string s = "", sql = "";

                        foreach (var B in ps)
                        {
                            sql = Reflection.GetDandaanColumnAttribute(B).Sql;

                            if (!SQL.IsForeignKey(sql) || (SQL.IsForeignKey(sql) && SQL.IsIdentity(sql)))
                                s += B.GetValue(result) + "  ";
                        }

                        button.DefaultText = button.Text = s.Trim();
                    }

                    /*var result = typeof(SQL).GetMethod(nameof(SQL.SelectAll)).MakeGenericMethod(type)
                    .Invoke(null, null);

                    foreach (var B in (System.Collections.IEnumerable)result)
                    {
                        var text = type.GetProperty(da.ForeignTableDisplayColumn).GetValue(B).ToString();
                        var value = type.GetProperty(SQL.GetForeignColumn(da.Sql)).GetValue(B);

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
                    }*/
                    
                    button.Click += (_, __) =>
                    {
                        var t = typeof(Forms.Browser<,>).MakeGenericType(new Type[] { type, typeof(ListView) });

                        var selectedObj = t.GetMethod(nameof(Forms.Browser<object, Control>.ShowAndReturnSelection))
                        .Invoke(Activator.CreateInstance(t), null);

                        (button as Controls.ButtonEdit).Obj = selectedObj;

                        if (selectedObj != null)
                        {
                            var ps = type.GetProperties();

                            string s = "", sql = "";

                            foreach (var B in ps)
                            {
                                sql = Reflection.GetDandaanColumnAttribute(B).Sql;

                                if (!SQL.IsForeignKey(sql) || (SQL.IsForeignKey(sql) && SQL.IsIdentity(sql)))
                                    s += B.GetValue(selectedObj) + "  ";
                            }

                            button.Text = s.Trim();
                        }
                    };
                }

                //
                
                var textChanged = new Func<Control, EventHandler>((c) => new EventHandler((_, __) =>
                {
                    if ((c is TextBox && c.Text != (c as Controls.TextBox).DefaultText)
                    || (c is ComboBox && c.Text != (c as Controls.ComboBox).DefaultText)
                    || (c is Button && c.Text != (c as Controls.ButtonEdit).DefaultText))
                    {
                        c.BackColor = Color.LightYellow;//LightGoldenrodYellow;//MistyRose;

                        if (kind != EditorKind.Search)
                            (form.AcceptButton as Button).Enabled = true;
                    }
                    else
                    {
                        if (c is TextBox)
                            c.BackColor = (c as Controls.TextBox).DefaultBackColor;
                        else if (c is ComboBox)
                            c.BackColor = (c as Controls.ComboBox).DefaultBackColor;
                        else if (c is Button)
                        {
                            c.BackColor = (c as Controls.ButtonEdit).DefaultBackColor;
                            (c as Controls.ButtonEdit).UseVisualStyleBackColor = true;
                        }

                        if (kind != EditorKind.Search)
                        {
                            (form.AcceptButton as Button).Enabled = false;

                            foreach (Control A in Controls)
                                if ((A is TextBox && A.Text != (A as Controls.TextBox).DefaultText && !(A as TextBox).ReadOnly)
                                || (A is ComboBox && A.Text != (A as Controls.ComboBox).DefaultText)
                                || (A is UserControl && A.Controls[A.Name + nameof(Dandaan.Controls.ButtonEdit)].Text != (A.Controls[A.Name + nameof(Dandaan.Controls.ButtonEdit)] as Controls.ButtonEdit).DefaultText))
                                    (form.AcceptButton as Button).Enabled = true;
                        }
                    }

                    if (kind == EditorKind.Search)
                    {
                        var contains = false;

                        if (c is ComboBox)
                        {
                            contains = (c as ComboBox).Items.Contains(c.Text);

                            if (!contains && (c as ComboBox).Items.Count > 0 && (c as ComboBox).Items[0] is ComboboxItem)
                                foreach (ComboboxItem B in (c as ComboBox).Items)
                                    if (B.Text == c.Text) { contains = true; break; }
                        }

                        if (!(c is ComboBox && !contains)) searchAct(getObj(propertyInfos, kind));
                    }
                }));

                //

                if (control is TextBox)
                    (control as TextBox).TextChanged += textChanged(control);
                else if (control is ComboBox)
                    (control as ComboBox).TextChanged += textChanged(control);
                else if (control is UserControl)
                    control.Controls[control.Name + nameof(Dandaan.Controls.ButtonEdit)].TextChanged 
                    += textChanged(control.Controls[control.Name + nameof(Dandaan.Controls.ButtonEdit)]);

                //

                if (xMargin + control.Width + margin > maxX) maxX = xMargin + control.Width + margin;
            };

            foreach (var item in propertyInfos)
            {
                var ut = Nullable.GetUnderlyingType(item.PropertyType);

                var da = Reflection.GetDandaanColumnAttribute(item);

                Control control = null;

                //

                if (item.PropertyType.IsEnum || (ut != null && ut.IsEnum))
                {
                    control = new Controls.ComboBox();
                }
                else if (SQL.IsForeignKey(da.Sql)/* || item.PropertyType == typeof(DateTime)
                    || (ut != null && ut == typeof(DateTime))*/)
                {
                    control = new UserControl();
                }
                else
                {
                    control = new Controls.TextBox();

                    control.MouseClick += (_, __) => (control as Controls.TextBox).SelectAll();
                }

                //

                Controls.Add(control);

                act(control, item, da);

                //

                var labelText = da.Label + ":";

                label = new Label()
                {
                    Text = labelText,
                    TextAlign = ContentAlignment.TopRight,
                    //BorderStyle = BorderStyle.FixedSingle,
                    Margin = new Padding(0),
                    Padding = new Padding(0),
                    //Anchor = AnchorStyles.None,
                    RightToLeft = RightToLeft.Yes,
                    AutoSize = true,
                };

                Controls.Add(label);

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

                y += (label.Height > control.Height ? label.Height : control.Height) + yMargin;
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
                //Anchor = AnchorStyles.None,
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
                //Anchor = AnchorStyles.None,
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
                        {
                            if (beforeAdd(obj)) SQL.Insert(obj);
                            else return;
                        }
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
                            else if (item is UserControl)
                            {
                                if (kind == EditorKind.Add)
                                    item.Controls[item.Name + nameof(Dandaan.Controls.ButtonEdit)].Text =
                                    (item.Controls[item.Name + nameof(Dandaan.Controls.ButtonEdit)] as Controls.ButtonEdit).DefaultText;
                                else
                                {
                                    (item.Controls[item.Name + nameof(Dandaan.Controls.ButtonEdit)] as Controls.ButtonEdit).DefaultText = (item.Controls[item.Name + nameof(Dandaan.Controls.ButtonEdit)] as Controls.ButtonEdit).Text;
                                    (item.Controls[item.Name + nameof(Dandaan.Controls.ButtonEdit)]).Text = (item.Controls[item.Name + nameof(Dandaan.Controls.ButtonEdit)] as Controls.ButtonEdit).Text + " ";
                                    (item.Controls[item.Name + nameof(Dandaan.Controls.ButtonEdit)]).Text = (item.Controls[item.Name + nameof(Dandaan.Controls.ButtonEdit)] as Controls.ButtonEdit).DefaultText;
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
                //Anchor = AnchorStyles.None,
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

                if (Controls[item.Name] is UserControl)
                {
                    var o = (Controls[item.Name].Controls[item.Name + nameof(Dandaan.Controls.ButtonEdit)] as Controls.ButtonEdit).Obj;
                    if (o != null)
                    {
                        var type = o.GetType();
                        var ps = type.GetProperties();
                        var property = ps.Where(_p => _p.Name == SQL.GetForeignColumn(Reflection.GetDandaanColumnAttribute(item).Sql)).First();
                        var value = property.GetValue(o);
                        item.SetValue(obj, value);
                    }
                }
                else if (Controls[item.Name] is ComboBox || p != null)
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

                if (p == null)
                {
                    p = item.GetType().GetProperty(nameof(Enabled));

                    if (!(item is Label) && p != null && (bool)p.GetValue(item)) { item.Select(); break; }
                }
                else if (!(bool)p.GetValue(item)) { item.Select(); break; }
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
