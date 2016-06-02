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
    public partial class ListViewBrowser<T> : Browser<T> where T : class
    {
        public ListViewBrowser()
        {
            InitializeComponent();

            Init(listViewBrowser1.browserMenu1);

            listViewBrowser1.ColumnsAct = () =>
            {
                using (var g = CreateGraphics())
                {
                    int sum = 0;
                    SizeF size;
                    foreach (var item in propertyInfos)
                    {
                        var label = Reflection.GetDandaanAttribute(item).Label;

                        size = g.MeasureString(label, Font);

                        listViewBrowser1.listView1.Columns.Add(label).Width = (int)size.Width + 10;

                        sum += (int)size.Width + 10;
                    }

                    if (listViewBrowser1.listView1.Width > sum && (listViewBrowser1.listView1.Width - sum) / propertyInfos.Length > 1)
                    {
                        var x = (listViewBrowser1.listView1.Width - sum) / propertyInfos.Length;

                        for (int i = 0; i < listViewBrowser1.listView1.Columns.Count; i++)
                            listViewBrowser1.listView1.Columns[i].Width = listViewBrowser1.listView1.Columns[i].Width
                                + ((propertyInfos[i].PropertyType == typeof(int)) ? 0 : x--);
                    }
                }
            };

            //bool odd = true;
            listViewBrowser1.ItemsFunc = () => {
                bool odd = true;
                return SQL.Select<T>(listViewBrowser1.browserMenu1.Page, listViewBrowser1.browserMenu1.PageSize)
                .Select((row) =>
                {
                    odd = !odd;
                    return new ListViewItem(propertyInfos.Select(item => item.GetValue(row).ToString()).ToArray())
                    { BackColor = odd ? Color.LightCyan : Color.FromArgb(0xcf, 0xff, 0xcf) };
                });
            };
        }
    }
}
