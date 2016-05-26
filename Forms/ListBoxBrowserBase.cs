﻿using System;
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
    public partial class ListBoxBrowserBase : Browser
    {
        public Func<IEnumerable<object>> ObjsFunc = () => new object[] { };

        public ListBoxBrowserBase()
        {
            InitializeComponent();

            // testing
            //browserMenu1.CountFunc = () => 1000;
            //var x = new List<object>(1000);
            //for (int i = 0; i < 1000; i++) x.Add(i);
            //ArrayFunc = () => { return x.Skip((browserMenu1.Page - 1) * browserMenu1.PageSize).Take(browserMenu1.PageSize).Select((k) => (object)(((int)k) + DateTime.Now.Second)).ToArray(); };
        }

        public Action Act(ListBox listBox)
        {
            return () =>
            {
                thread = Common.Thread(() =>
                {
                    var objs = ObjsFunc().ToArray();

                    Invoke(new Action(() =>
                    {
                        if (objs == null || objs.Length == 0)
                        {
                            listBox.Items.Clear();
                            listBox.Items.Add("رکوردی وجود ندارد.");
                        }
                        else
                        {
                            if (listBox.Items.Count == 0) listBox.Items.AddRange(objs);
                            else
                            {
                                for (int i = 0; i < listBox.Items.Count; i++)
                                    if (objs.Length > i)
                                    {
                                        if (listBox.Items[i].ToString() != objs[i].ToString())
                                        {
                                            if (listBox.Visible)
                                            {
                                                listBox.Hide();
                                                //browserMenu1.label2.Refresh();
                                                //browserMenu1.label3.Refresh();
                                            }

                                            listBox.Items[i] = objs[i];

                                            if (listBox.SelectedIndex == i) listBox.ClearSelected();

                                            if (listBox is CheckedListBox && (listBox as CheckedListBox).GetItemChecked(i))
                                                (listBox as CheckedListBox).SetItemChecked(i, false);
                                        }
                                    }
                                    else
                                    {
                                        if (listBox.Visible) listBox.Hide();
                                        while (listBox.Items.Count > i) listBox.Items.RemoveAt(i);
                                        break;
                                    }

                                if (listBox.Items.Count < objs.Length)
                                {
                                    if (listBox.Visible) listBox.Hide();
                                    for (int i = listBox.Items.Count; i < objs.Length; i++)
                                        listBox.Items.Add(objs[i]);
                                }

                                /*if (listBox.SelectedIndex < 0)
                                {
                                    listBox.SelectedIndex = 0;
                                    listBox.ClearSelected();
                                }*/

                                if (!listBox.Visible) listBox.Show();
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
