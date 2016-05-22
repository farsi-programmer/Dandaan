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
    public partial class ListBoxBrowserBase : Browser
    {
        public Func<object[]> ArrayFunc = () => null;

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
                new System.Threading.Thread(() =>
                {
                    var objs = ArrayFunc();

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
                                            listBox.Items[i] = objs[i];

                                            if (listBox.SelectedIndex == i) listBox.ClearSelected();

                                            if (listBox is CheckedListBox && ((CheckedListBox)listBox).GetItemChecked(i))
                                                ((CheckedListBox)listBox).SetItemChecked(i, false);
                                        }
                                    }
                                    else listBox.Items.RemoveAt(i);

                                for (int i = listBox.Items.Count; i < objs.Length; i++)
                                    listBox.Items.Add(objs[i]);

                                /*if (listBox.SelectedIndex < 0)
                                {
                                    listBox.SelectedIndex = 0;
                                    listBox.ClearSelected();
                                }*/
                            }
                        }
                    }));
                }).Start();
            };
        }
    }
}
