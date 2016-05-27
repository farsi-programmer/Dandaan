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
    public partial class DataGridViewBrowser : Browser
    {
        public Func<DataGridViewRow[]> ArrayFunc = () => null;

        public DataGridViewBrowser()
        {
            InitializeComponent();

            // testing
            browserMenu1.CountFunc = () => 1000;
            var x = new List<DataGridViewRow>(1000);
            for (int i = 0; i < 1000; i++) { var r = new DataGridViewRow(); r.Cells.Add(new DataGridViewTextBoxCell() { Value = i }); x.Add(r); }
            ArrayFunc = () => { return x.Skip((browserMenu1.Page - 1) * browserMenu1.PageSize).Take(browserMenu1.PageSize).Select((k) => { Invoke(new Action(() => { k.Cells[0].Value = (int)k.Cells[0].Value + DateTime.Now.Second; })); return k; }).ToArray(); };
            dataGridView1.Columns.Add("test", "test");
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            //
            // datagridview is complicated, this is incomplete!
            //

            browserMenu1.Act = () =>
            {
                thread = Common.Thread(() =>
                {
                    var objs = ArrayFunc();

                    Invoke(new Action(() =>
                    {
                        if (objs == null || objs.Length == 0)
                        {
                            dataGridView1.Columns.Clear();
                            dataGridView1.Columns.Add("", "");
                            dataGridView1.Rows.Clear();
                            dataGridView1.Rows.Add("رکوردی وجود ندارد.");
                            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                        }
                        else
                        {
                            if (dataGridView1.Rows.Count == 0 || (dataGridView1.Rows.Count == 1
                            && dataGridView1.Rows[0].Cells.Count == 1
                            && dataGridView1.Rows[0].Cells[0].Value == null))
                                dataGridView1.Rows.AddRange(objs);
                            else
                            {
                                /*for (int i = 0; i < dataGridView1.Rows.Count; i++)
                                    if (objs.Length > i)
                                    {
                                        //foreach (var item in dataGridView1.Rows[i].Cells)
                                        {
                                            if (dataGridView1.Rows[i].Cells[0].Value != objs[i].Cells[0].Value)
                                                dataGridView1.Rows[i].Cells[0].Value = objs[i].Cells[0].Value;
                                        }
                                    }
                                    else dataGridView1.Rows.RemoveAt(i);

                                for (int i = dataGridView1.Rows.Count; i < objs.Length; i++)
                                    dataGridView1.Rows.Add(objs[i]);*/
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
