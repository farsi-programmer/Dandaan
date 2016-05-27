using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dandaan.UserControls
{
    public partial class ListViewBrowser : Browser
    {
        public Func<IEnumerable<ListViewItem>> ItemsFunc = () => new ListViewItem[] { };

        public ListViewBrowser()
        {
            InitializeComponent();

            // testing
            //browserMenu1.CountFunc = () => 1000;
            //var x = new List<ListViewItem>(1000);
            //for (int i = 0; i < 1000; i++) x.Add(new ListViewItem(i.ToString()));
            //ItemsFunc = () => { return x.Skip((browserMenu1.Page - 1) * browserMenu1.PageSize).Take(browserMenu1.PageSize).Select((k) => { Invoke(new Action(() => k.Text = k.Text + DateTime.Now.Second)); /*new ListViewItem(k.Text + DateTime.Now.Second)*/ return k; }).ToArray(); };
            //listView1.Columns.Add("test", 200);

            browserMenu1.Act = () =>
            {
                thread = Common.Thread(() =>
                {
                    var items = ItemsFunc().ToArray();

                    Invoke(new Action(() =>
                    {
                        if (items == null || items.Length == 0)
                        {
                            listView1.Columns.Clear();
                            listView1.Columns.Add("");

                            listView1.Items.Clear();
                            listView1.Items.Add("رکوردی وجود ندارد.");

                            listView1.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                        }
                        else
                        {
                            if (listView1.Items.Count == 0) listView1.Items.AddRange(items);
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
                                            }
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

                        browserMenu1.enable();
                    }));
                });
                
                thread.Start();
            };
        }
    }
}
