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
    public partial class Editor<T> : Form where T : class
    {
        public Editor()
        {
            InitializeComponent();

            AutoScroll = true;
        }

        public const string _info_ = "_info_";

        public Editor(PropertyInfo[] propertyInfos) : this()
        {
            int y = 15, maxX = 0, textBoxWidth = 350, margin = 2, xMargin = 15, tabIndex = 0;
            Label label;

            foreach (var item in propertyInfos)
            {
                var da = Reflection.GetDandaanAttribute(item);

                //if (item.PropertyType == typeof(string))
                {
                    //

                    var textBox = new TextBox()
                    {
                        Width = textBoxWidth,
                        Margin = new Padding(0),
                        Padding = new Padding(0),
                        Location = new Point(xMargin, y),
                        ReadOnly = Common.IsMatch(da.Sql, @"[\s]+identity[\s]+"),
                        RightToLeft = RightToLeft.Yes,
                        Name = item.Name,
                        TabIndex = tabIndex++,
                    };

                    var color = textBox.BackColor;
                    textBox.TextChanged += (_, __) =>
                    {
                        if (textBox.Text != "")
                        {
                            textBox.BackColor = Color.LightYellow;//LightGoldenrodYellow;//MistyRose;
                            (AcceptButton as Button).Enabled = true;
                        }
                        else
                        {
                            textBox.BackColor = color;
                            (AcceptButton as Button).Enabled = false;
                            foreach (Control A in Controls)
                                if (A is TextBox && A.Text != "") (AcceptButton as Button).Enabled = true;
                        }
                    };

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

            var buttonCancel = new Button() { Text = "انصراف", AutoSize = true, TabIndex = tabIndex + 1, };
            buttonCancel.Location = new Point(maxX - buttonCancel.Width - 5, y);
            buttonCancel.Click += (_, __) => Close();
            Controls.Add(buttonCancel);
            CancelButton = buttonCancel;

            //

            var buttonAccept = new Button() { Text = "اضافه", AutoSize = true, TabIndex = tabIndex++, Enabled = false };
            buttonAccept.Location = new Point(buttonCancel.Location.X - buttonAccept.Width - 8, y);
            buttonAccept.Click += (_, __) =>
            {
                var obj = Activator.CreateInstance<T>();
                //bool blank = true;

                foreach (var item in propertyInfos)
                {
                    var p = Controls[item.Name].GetType().GetProperty(nameof(TextBox.ReadOnly));

                    if (p != null && !(bool)p.GetValue(Controls[item.Name]))
                    {
                        item.SetValue(obj, Controls[item.Name].Text);
                        //if (Controls[item.Name].Text != "") blank = false;
                    }
                }

                //if (blank) MessageBox.Show("لطفا ");
                //else
                {
                    SQL.Insert(obj);

                    Controls[_info_].ForeColor = Controls[_info_].ForeColor == Color.Green ? Color.SlateBlue : Color.Green;
                    Controls[_info_].Text = "اضافه شد";

                    foreach (Control item in Controls) if (item is TextBox) item.Text = "";
                }
            };

            Controls.Add(buttonAccept);
            AcceptButton = buttonAccept;

            tabIndex++;

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

        private void Editor_Load(object sender, EventArgs e)
        {
            foreach (Control item in Controls)
            {
                var p = item.GetType().GetProperty(nameof(TextBox.ReadOnly));
                if (p != null && !(bool)p.GetValue(item))
                { item.Select(); break; }
            }
        }
    }
}
