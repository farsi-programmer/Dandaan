using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dandaan
{
    public partial class FormDB : Form
    {
        public FormDB()
        {
            InitializeComponent();
        }

        Thread threadDB;

        private void FormDB_Load(object sender, EventArgs e)
        {
            connect();
        }

        void connect()
        { 
            // i want something responsive, in case it takes
            // a long time to connect to db

            textBox1.AppendText("در حال اتصال به دیتابیس، لطفا صبر کنید...\r\n");

            threadDB = new Thread(() =>
            {
                try
                {
                    DB.Init();

                    /*Parallel.For(0, 10, new Action<int>((j) =>
                    {
                        for (int i = 0; i < 10; i++)
                            Invoke(new Action(() => textBox2.AppendText(DB.ExecuteScalar(
                                //@"select @@version"
                                @"select name FROM master.dbo.sysdatabases ORDER BY Name OFFSET 0 ROWS"
                            ).ToString())));
                    }));*/

                    try
                    {
                        Invoke(new Action(() =>
                        {
                            try//this is another thread
                            {
                                textBox1.AppendText("اتصال برقرار شد.\r\n");

                                Visible = false;
                                new FormMain().ShowDialog(this);
                                Close();
                            }
                            catch { }
                        }));
                    }
                    catch { }
                }
                catch (ThreadAbortException) { }
                catch (Exception ex)
                {
                    try
                    {
                        Invoke(new Action(() =>
                        {
                            try//this is another thread
                            {
                                textBox1.AppendText("خطا در برقراری ارتباط با دیتابیس!\r\n");
                                textBox2.AppendText(ex.Message + "\r\n");
                                button1.Enabled = true;
                            }
                            catch { }
                        }));
                    }
                    catch { }
                }
            });

            threadDB.Start();
        }

        private void FormDB_FormClosing(object sender, FormClosingEventArgs e)
        {
            threadDB?.Abort();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            connect();
        }
    }
}
