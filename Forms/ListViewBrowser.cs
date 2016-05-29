using System;
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
    public partial class ListViewBrowser<T> : Form where T : class
    {
        public ListViewBrowser()
        {
            InitializeComponent();

            Text = Reflection.GetDandaanAttribute(typeof(T)).Label;

            listViewBrowser1.browserMenu1.CountFunc = SQL.Count<T>;

            var ps = typeof(T).GetProperties();

            using (var g = CreateGraphics())
            {
                int sum = 0;
                SizeF size;
                foreach (var item in ps)
                {
                    var label = Reflection.GetDandaanAttribute(item).Label;

                    size = g.MeasureString(label, Font);

                    listViewBrowser1.listView1.Columns.Add(label).Width = (int)size.Width + 10;

                    sum += (int)size.Width + 10;
                }

                if (listViewBrowser1.listView1.Width > sum && (listViewBrowser1.listView1.Width - sum) / ps.Length > 1)
                {
                    var x = (listViewBrowser1.listView1.Width - sum) / ps.Length;

                    for (int i = 0; i < listViewBrowser1.listView1.Columns.Count; i++)
                        listViewBrowser1.listView1.Columns[i].Width = listViewBrowser1.listView1.Columns[i].Width
                            + ((ps[i].PropertyType == typeof(int)) ? 0 : x--);
                }
            }

            bool odd = true;
            listViewBrowser1.ItemsFunc = () =>
            SQL.Select<T>(listViewBrowser1.browserMenu1.Page, listViewBrowser1.browserMenu1.PageSize)
            .Select((row) =>
            {
                odd = !odd;
                return new ListViewItem(ps.Select(item => item.GetValue(row).ToString()).ToArray())
                { BackColor = odd ? Color.LightCyan : Color.FromArgb(0xcf, 0xff, 0xcf) };
            });

            listViewBrowser1.browserMenu1.AddAct = () =>
            {
                ;
            };
        }
    }
}
