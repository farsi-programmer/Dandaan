using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Dandaan.Forms
{
    // for designer
    class _About : About { }

    class About : Form
    {
        public About()
        {
            Text = "درباره";

            //

            int y = 20, z = 20;

            Action<Label> act = (label) =>
            {
                // for some reason creating the form's graphics object, before its size is set
                // results in it not being on centerscreen, so we are using the label itself
                using (var g = label.CreateGraphics())
                    label.Size = g.MeasureString(label.Text, Font).ToSize();

                label.Location = new Point(z, y);

                Controls.Add(label);

                y += label.Size.Height + 20;
            };

            //

            var label1 = new Label() { Text = "نرم افزار مدیریت دندانپزشکی متن باز" };
            act(label1);

            //

            var linkLabel1 = new LinkLabel() { Text = "http://offtopic.blog.ir/", TabStop = true };
            act(linkLabel1);
            linkLabel1.LinkClicked += (_, __) => Process.Start(linkLabel1.Text);

            //

            var linkLabel2 = new LinkLabel()
            {
                Text = "https://github.com/farsi-programmer/Dandaan",
                TabStop = true
            };
            act(linkLabel2);
            linkLabel2.LinkClicked += (_, __) => Process.Start(linkLabel2.Text);

            //

            var w = 0;
            foreach (Control item in Controls) if (item.Size.Width > w) w = item.Size.Width;

            foreach (Control item in Controls)
                if (w > item.Size.Width)
                    item.Location = new Point(item.Location.X + (w - item.Size.Width) / 2, item.Location.Y);

            //

            ClientSize = new Size(w + z, y + 5);
        }
    }
}
