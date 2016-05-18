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
    public partial class ListBrowser : UserControl
    {
        public Func<object[]> ArrayFunc = () => null;

        public ListBrowser()
        {
            InitializeComponent();

            browserMenu1.ChangeFocus = listBox1.Focus;

            // testing
            //browserMenu1.CountFunc = () => 1000;
            //var x = new List<object>(1000);
            //for (int i = 0; i < 1000; i++) x.Add(i);
            //ArrayFunc = () => { return x.Skip((browserMenu1.Page - 1) * browserMenu1.PageSize).Take(browserMenu1.PageSize).Select((k)=>(object)(((int)k)+DateTime.Now.Second)).ToArray(); };
            
            browserMenu1.Act = () =>
            {
                new System.Threading.Thread(() =>
                {
                    var objs = ArrayFunc();

                    Invoke(new Action(() =>
                    {
                        if (objs == null || objs.Length == 0)
                        {
                            listBox1.Items.Clear();
                            listBox1.Items.Add("رکوردی وجود ندارد.");
                        }
                        else
                        {
                            if (listBox1.Items.Count == 0) listBox1.Items.AddRange(objs);
                            else
                            {
                                for (int i = 0; i < listBox1.Items.Count; i++)
                                    if (objs.Length > i)
                                    {
                                        if (listBox1.Items[i].ToString() != objs[i].ToString())
                                        {
                                            listBox1.Items[i] = objs[i];

                                            if (listBox1.SelectedIndex == i) listBox1.ClearSelected();
                                        }
                                    }
                                    else listBox1.Items.RemoveAt(i);

                                for (int i = listBox1.Items.Count; i < objs.Length; i++)
                                    listBox1.Items.Add(objs[i]);

                                if (listBox1.SelectedIndex < 0)
                                {
                                    listBox1.SelectedIndex = 0;
                                    listBox1.ClearSelected();
                                }
                            }
                        }

                        browserMenu1.Working = false;
                    }));
                }).Start();
            };
        }
    }
}
