﻿using System;
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
        protected Func<IEnumerable<ListViewItem>> ItemsFunc;
        protected Action ColumnsAct;

        const string NoRecords = "رکوردی وجود ندارد.";
        const string Blank = "";

        public ListViewBrowser()
        {
            InitializeComponent();

            ColumnsAct = () =>
            {
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
            };

            //bool odd = true;
            ItemsFunc = () => {
                bool odd = true;
                return SQL.Select<T>(Page, PageSize)
                .Select((row) =>
                {
                    odd = !odd;
                    return new ListViewItem(PropertyInfos.Select(item => item.GetValue(row).ToString()).ToArray())
                    { BackColor = odd ? Color.LightCyan : Color.FromArgb(0xcf, 0xff, 0xcf) };
                });
            };
        }

        protected override void Act()
        {
            // testing
            //browserMenu1.CountFunc = () => 1000;
            //var x = new List<ListViewItem>(1000);
            //for (int i = 0; i < 1000; i++) x.Add(new ListViewItem(i.ToString()));
            //ItemsFunc = () => { return x.Skip((browserMenu1.Page - 1) * browserMenu1.PageSize).Take(browserMenu1.PageSize).Select((k) => { Invoke(new Action(() => k.Text = k.Text + DateTime.Now.Second)); /*new ListViewItem(k.Text + DateTime.Now.Second)*/ return k; }).ToArray(); };
            //listView1.Columns.Add("test", 200);

            thread = Common.Thread(() =>
            {
                var items = ItemsFunc().ToArray();

                Invoke(() =>
                {
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

                            ColumnsAct();
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

                    Enable();
                });
            });

            thread.Start();
        }
    }
}
