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

        public Editor(PropertyInfo[] propertyInfos) : this()
        {
            int y = 15, maxX = 0, textBoxWidth = 350, margin = 2, xMargin = 15, tabIndex = 0;

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

                    if (xMargin + textBox.Size.Width + margin > maxX) maxX = xMargin + textBox.Size.Width + margin;

                    //

                    var labelText = da.Label + ":";

                    var label = new Label()
                    {
                        Text = labelText,
                        TextAlign = ContentAlignment.TopRight,
                        //BorderStyle = BorderStyle.FixedSingle,
                        Margin = new Padding(0),
                        Padding = new Padding(0),
                        RightToLeft = RightToLeft.Yes,
                    };

                    Size s;

                    using (var g = label.CreateGraphics())
                        s = g.MeasureString(labelText, Font).ToSize();

                    label.Size = new Size(s.Width + 5, s.Height);

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
                else item.Location = new Point(maxX - item.Size.Width - margin, item.Location.Y);

            foreach (Control item in Controls)
                if (item.Size.Width + item.Location.X > maxX) maxX = item.Size.Width + item.Location.X;

            //

            y += 5;

            var buttonCancel = new Button() { Text = "انصراف", AutoSize = true, TabIndex = tabIndex + 1, };
            buttonCancel.Location = new Point(maxX - buttonCancel.ClientSize.Width - 5, y);
            buttonCancel.Click += (_, __) => Close();
            Controls.Add(buttonCancel);
            CancelButton = buttonCancel;

            //

            var buttonAccept = new Button() { Text = "اضافه", AutoSize = true, TabIndex = tabIndex++, };
            buttonAccept.Location = new Point(buttonCancel.Location.X - buttonAccept.ClientSize.Width - 8, y);
            buttonAccept.Click += (_, __) =>
            {
                var obj = Activator.CreateInstance<T>();
                foreach (var item in propertyInfos)
                {
                    var p = Controls[item.Name].GetType().GetProperty(nameof(TextBox.ReadOnly));
                    if (p != null && !(bool)p.GetValue(Controls[item.Name]))
                        item.SetValue(obj, Controls[item.Name].Text);
                }
                SQL.Insert(obj);
            };

            //

            tabIndex++;

            Controls.Add(buttonAccept);
            AcceptButton = buttonAccept;

            y += buttonAccept.ClientSize.Height;

            //

            ClientSize = new Size(maxX + 5, y + 12);
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
