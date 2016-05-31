using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dandaan.Forms
{
    class _ConnectDB : ConnectDB { }

    class ConnectDB : Form
    {
        TextBox textBox1, textBox2;
        Button button1;

        public ConnectDB()
        {
            //MessageBox.Show((LicenseManager.UsageMode == LicenseUsageMode.Designtime).ToString());

            Text = "اتصال به دیتابیس";

            ClientSize = new Size((int)(ClientSize.Width / 1.2), (int)(ClientSize.Height / 1.2));

            //

            int y = 8, z = 8, x = 8;

            textBox1 = new TextBox()
            {
                Multiline = true,
                ReadOnly = true,
                RightToLeft = RightToLeft.Yes,
                ScrollBars = ScrollBars.Both,
                Location = new Point(x, y),
                ClientSize = new Size(Width - (2 * x), 100),
            };

            y += textBox1.Size.Height + z;

            Controls.Add(textBox1);

            //

            textBox2 = new TextBox()
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                Size = new Size(Width - 20, 150),
                Location = new Point(x, y),
            };

            y += textBox2.Size.Height + z;

            Controls.Add(textBox2);

            //

            button1 = new Button()
            {
                Enabled = false,
                Location = new Point(457, 458),
                Size = new Size(106, 44),
                Text = "تلاش مجدد",
                UseVisualStyleBackColor = true,
            };

            button1.Click += (_, __) =>
            {
                button1.Enabled = false;
                connect();
            };

            Controls.Add(button1);

            //

            var button2 = new Button()
            {
                Location = new Point(569, 459),
                Size = new Size(106, 42),
                Text = "انصراف",
                UseVisualStyleBackColor = true,
            };

            button2.Click += (_, __) => Close();

            Controls.Add(button2);

            //

            CancelButton = button2;

            FormClosing += (_, __) => thread?.Abort();

            Load += (_, __) => connect();
        }

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
                            if (new Login().ShowDialog() == DialogResult.OK)
                                new Main().ShowDialog();

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
                        Invoke(new Action(() =>
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
                        }));
                    }
                    catch { } // in case the application is being closed and the form is disposed
                }
            });

            thread.Start();
        }
    }
}
