using System;
using System.Text.RegularExpressions;
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
    public partial class ListBoxBrowserBase<T> : Browser<T> where T : class
    {
        protected Func<IEnumerable<object>> ObjsFunc;

        protected ListBox ListBox;

        public ListBoxBrowserBase()
        {
            InitializeComponent();

            // testing
            //browserMenu1.CountFunc = () => 1000;
            //var x = new List<object>(1000);
            //for (int i = 0; i < 1000; i++) x.Add(i);
            //ObjsFunc = () => { return x.Skip((browserMenu1.Page - 1) * browserMenu1.PageSize).Take(browserMenu1.PageSize).Select((k) => (object)(((int)k) + DateTime.Now.Second)).ToArray(); };

            ObjsFunc = () =>
            SQL.Select<T>(Page, PageSize)
            .Select((row) =>
            {
                var sb = new StringBuilder();
                foreach (var item in PropertyInfos)
                {
                    var obj = item.GetValue(row);

                    if (obj is string) obj = Regex.Replace(obj as string, "[\r\n]+", " ");

                    sb.Append(obj + "\t");
                }
                return sb.ToString();
            });
        }

        protected override void LoadAct()
        {
            thread = Common.Thread(() =>
            {
                var objs = ObjsFunc().ToArray();

                Invoke(() =>
                {
                    if (objs == null || objs.Length == 0)
                    {
                        ListBox.Items.Clear();
                        ListBox.Items.Add("رکوردی وجود ندارد.");
                    }
                    else
                    {
                        if (ListBox.Items.Count == 0) ListBox.Items.AddRange(objs);
                        else
                        {
                            for (int i = 0; i < ListBox.Items.Count; i++)
                                if (objs.Length > i)
                                {
                                    if (ListBox.Items[i].ToString() != objs[i].ToString())
                                    {
                                        if (ListBox.Visible) ListBox.Hide();

                                        ListBox.Items[i] = objs[i];

                                        if (ListBox.SelectedIndex == i) ListBox.ClearSelected();

                                        if (ListBox is CheckedListBox && (ListBox as CheckedListBox).GetItemChecked(i))
                                            (ListBox as CheckedListBox).SetItemChecked(i, false);
                                    }
                                }
                                else
                                {
                                    if (ListBox.Visible) ListBox.Hide();
                                    while (ListBox.Items.Count > i) ListBox.Items.RemoveAt(i);
                                    break;
                                }

                            if (ListBox.Items.Count < objs.Length)
                            {
                                if (ListBox.Visible) ListBox.Hide();
                                for (int i = ListBox.Items.Count; i < objs.Length; i++)
                                    ListBox.Items.Add(objs[i]);
                            }

                            /*if (ListBox.SelectedIndex < 0)
                            {
                                ListBox.SelectedIndex = 0;
                                ListBox.ClearSelected();
                            }*/

                            if (!ListBox.Visible) ListBox.Show();
                        }
                    }

                    Enable();
                });
            });

            thread.Start();
        }

        protected override void DeleteAct()
        {
            ;
        }
    }
}
