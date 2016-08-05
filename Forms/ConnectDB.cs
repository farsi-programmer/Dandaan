using System;
using System.Threading;
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
    // for designer
    //public class _ConnectDB : ConnectDB { }

    public partial class ConnectDB : Form
    {
        public ConnectDB()
        {
            InitializeComponent();

            CancelButton = button2;

            //MessageBox.Show((LicenseManager.UsageMode == LicenseUsageMode.Designtime).ToString());
        }

        private void ConnectDB_FormClosing(object sender, FormClosingEventArgs e) => thread?.Abort();

        private void ConnectDB_Load(object sender, EventArgs e) => connect();

        Thread thread;

        private void connect()
        {
            //MessageBox.Show(DesignMode.ToString());

            Program.ReadLocalSettings();

            // i want something responsive, in case it takes
            // a long time to connect to db

            textBox1.AppendText("در حال اتصال به دیتابیس، لطفا اندکی صبر کنید...\r\n");

            // we use threads when we don't want to block the UI thread,
            // or when we can speed up a task by using multiple threads
            // (splitting it between multiple threads)

            thread = new Thread(() =>
            {
                int tries = 0;
                go:
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
                        Invoke(() =>//(Action)delegate()
                        {
                            // this is in the main thread

                            try
                            {
                                textBox1.AppendText("اتصال برقرار شد.\r\n");
                                Visible = false;
                            }
                            catch { }

                            // i want to see errors
                            if (new Login().ShowDialog() == DialogResult.OK) new Main().ShowDialog();

                            try
                            {
                                Close();
                            }
                            catch { }
                        });
                    }
                    catch { }
                }
                catch (ThreadAbortException) { }
                catch (Exception ex)
                {
                    while (ex.InnerException != null) ex = ex.InnerException;

                    // System.Data.SqlClient.SqlException (0x80131904)
                    if (tries < 1 && ex.Message.Contains("Login failed for user")) { tries++; goto go; }

                    try
                    {
                        Invoke(() =>
                        {
                            try // this is in the main thread
                            {
                                textBox1.AppendText("خطا در برقراری ارتباط با دیتابیس!\r\n");
                                textBox2.AppendText(ex + "\r\n\r\n");
                                //MessageBox.Show(ex.ToString());
                                button1.Enabled = true;
                                DB.Log(ex.ToString());
                            }
                            catch { }
                        });
                    }
                    catch { } // in case the application is being closed and the form is disposed
                }
            });

            thread.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            connect();
        }

        private void button2_Click(object sender, EventArgs e) => Close();
    }
}
