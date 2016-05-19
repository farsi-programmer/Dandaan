using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dandaan.Forms
{
    public partial class ListViewBrowser : Browser
    {
        public Func<ListViewItem[]> ArrayFunc = () => null;

        public ListViewBrowser()
        {
            InitializeComponent();

            // testing
            //browserMenu1.CountFunc = () => 1000;
            //var x = new List<ListViewItem>(1000);
            //for (int i = 0; i < 1000; i++) x.Add(new ListViewItem(i.ToString()));
            //ArrayFunc = () => { return x.Skip((browserMenu1.Page - 1) * browserMenu1.PageSize).Take(browserMenu1.PageSize).Select((k) => new ListViewItem(k.Text + DateTime.Now.Second)).ToArray(); };
            //listView1.Columns.Add("test", 200);

            browserMenu1.ChangeFocus = listView1.Focus;

            browserMenu1.Act = () =>
            {
                new System.Threading.Thread(() =>
                {
                    var objs = ArrayFunc();

                    Invoke(new Action(() =>
                    {
                        if (objs == null || objs.Length == 0)
                        {
                            listView1.Columns.Clear();
                            listView1.Columns.Add("", 200);
                            listView1.Items.Clear();
                            listView1.Items.Add("رکوردی وجود ندارد.");
                            listView1.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                        }
                        else
                        {
                            if (listView1.Items.Count == 0) listView1.Items.AddRange(objs);
                            else
                            {
                                for (int i = 0; i < listView1.Items.Count; i++)
                                    if (objs.Length > i)
                                    {
                                        if (listView1.Items[i].Text != objs[i].Text)
                                            listView1.Items[i] = objs[i];
                                    }
                                    else listView1.Items.RemoveAt(i);

                                for (int i = listView1.Items.Count; i < objs.Length; i++)
                                    listView1.Items.Add(objs[i]);

                                /*if (listView1.SelectedIndices.Count == 0)
                                    listView1.EnsureVisible(0);*/
                            }
                        }

                        browserMenu1.Working = false;
                    }));
                }).Start();
            };
        }
    }
}
