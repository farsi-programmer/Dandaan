﻿// http://offtopic.blog.ir/

using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
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
    public partial class Main : Form
    {
        public Main()
        {
            // order is important

            InitializeComponent();            

            //Width += 30;
            //Height += 70;
            StartPosition = FormStartPosition.WindowsDefaultLocation;

#if using_ef || using_sqlite
            DB.Run((c) =>
            {
                var s = c.Settings.FirstOrDefault();

                if (s != null)
                {
                    if (s.FormMainWindowState != FormWindowState.Minimized)
                        WindowState = s.FormMainWindowState;
                }
            });
#else
            var s = SQL.SelectOrInsert(new Tables.Setting(false) { UserId = Program.UserId },
                new Tables.Setting(true) { UserId = Program.UserId });
            if ((FormWindowState)s.MainFormWindowState != FormWindowState.Minimized)
                WindowState = (FormWindowState)s.MainFormWindowState;
#endif
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
#if using_ef || using_sqlite
            DB.Run((c) =>
            {          
                if (c.Settings.Count() == 0)
                {
                    c.Settings.Add(new Setting() { FormMainWindowState = WindowState });
                }
                else
                    c.Settings.First().FormMainWindowState = WindowState;

                c.SaveChanges();
            });
#else
            Func<Tables.Setting> fun = () => SQL.SelectOrInsert(new Tables.Setting(false)
            { UserId = Program.UserId }, new Tables.Setting(true) { UserId = Program.UserId });

            var s = fun();
            s.MainFormWindowState = (int)WindowState;

            SQL.Update(s, fun());
#endif

            for (int i = 0; i < Application.OpenForms.Count; i++)
                if (Application.OpenForms[i] != this)
                    Application.OpenForms[i].Invoke(new Action(() => Application.OpenForms[i].Close()));
        }

        private void Main_Load(object sender, EventArgs e)
        {
            AddButtons();
        }

        List<string> userButtons;
        Dictionary<string, Form> userForms;

        public void AddButtons()
        { 
            int margin = 6, left = ClientSize.Width, bottom = ClientSize.Height,
                w = buttonPatients.Width, h = buttonPatients.Height;

            Action<Control> act = item =>
            {
                if (item.Location.X < left || item.Location.X == left)
                {
                    left = item.Location.X;
                    bottom = item.Location.Y + item.Height + margin;
                }

                if (bottom + h > ClientSize.Height)
                {
                    left = left - w - margin;
                    bottom = buttonPatients.Location.Y;
                }
            };

            if (userButtons != null)
            {
                foreach (var item in userButtons)
                {
                    var c = Controls[item];
                    Controls.RemoveByKey(item);
                    c.Dispose();
                }

                userButtons.Clear();
            }
            else userButtons = new List<string>();

            foreach (Control item in Controls) if (item is Button) act(item);

            //

            foreach (var userTable in DB.DataContext.UserTables.OrderBy(_ => _.Id))
            {
                Button button = new Button()
                {
                    Text = userTable.Label,
                    Location = new Point(left, bottom),
                    Anchor = buttonPatients.Anchor,
                    Width = w,
                    Height = h,
                };

                button.Click += (_, __) =>
                {
                    var name = $"{typeof(Tables.UserTable).FullName}{userTable.Id}";
                    var path = $"{Program.DataDirectory}\\{name}.dll";
                    var contextName = nameof(DataContext) + userTable.Id;

                    Action B = () =>
                    {
                        Type t = Assemblies.Load(path).GetType(name);

                        Form form = null;

                        Action C = () =>
                        {
                            form = (Form)Activator.CreateInstance(
                                typeof(Browser<,>).MakeGenericType(new[] { t, typeof(ListView) }));
                        };

                        if (userForms == null) userForms = new Dictionary<string, Form>();

                        if (userForms.ContainsKey(button.Text))
                        {
                            form = userForms[button.Text];

                            if (form.IsDisposed) C();
                        }
                        else
                        {
                            SQL.CreateTable(t);

                            C();

                            userForms.Add(button.Text, form);
                        }

                        ShowForm(ref form);
                    };

                    if (Assemblies.FromDbToFile(userTable.Id.Value, path)) B();
                    else
                    {
                        var unbuiltReferences = false;
                        var unbuiltLabel = "";

                        foreach (var A in DB.DataContext.Columns.Where(c => c.UserTableId == userTable.Id))
                            if (A.ColumnId != null)
                            {
                                var userTableId = DB.DataContext.Columns.Where(c =>
                                c.Id == A.ColumnId.Value).First().UserTableId;

                                var n = $"{nameof(Dandaan)}.{nameof(Tables)}.{nameof(Tables.UserTable)}{userTableId}";
                                var p = $"{Program.DataDirectory}\\{n}.dll";

                                if (userTable.Id != userTableId && !Assemblies.FromDbToFile(userTableId.Value, p))
                                {
                                    unbuiltReferences = true;

                                    unbuiltLabel = DB.DataContext.UserTables.Where(utb =>
                                    utb.Id == userTableId).First().Label;
                                    
                                    break;
                                }
                            }

                        if (unbuiltReferences)
                            MessageBox.Show($"این فرم به فرم:‏\r\n{unbuiltLabel}\r\nارجاع داده است، لطفا ابتدا آنرا بسازید.‏",
                                Program.Title);
                        else if (DB.DataContext.Columns.Where(c => c.UserTableId == userTable.Id).Count() == 0)
                            MessageBox.Show("به این فرم هیچ فیلدی اضافه نشده است، لطفا ابتدا یک یا چند فیلد به آن اضافه کنید.‏",
                                Program.Title);
                        else if (MessageBox.Show("این فرم هنوز ساخته نشده است، در صورت ساختن آن امکان تغییر دادن این فرم یا فیلدهای آن دیگر وجود نخواهد داشت!‏",
                            Program.Title, MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            new FormBuilder(path, name, userTable);
                            B();
                        }
                    }
                };

                button.Name = GetHashCode().ToString();
                userButtons.Add(button.Name);
                Controls.Add(button);
                act(button);
            }

            AutoScrollMinSize = new Size(ClientSize.Width - left + margin, AutoScrollMinSize.Height);
        }

        private void خروجToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new Thread(new ThreadStart(() =>
            Parallel.For(0, 100, new ParallelOptions() { MaxDegreeOfParallelism = 100 }, new Action<int>((i) =>
            {
#if using_ef || using_sqlite
                DB.Run((c) => FormLogger.Log("تست " + c.Logs.Count()));
#else
                SQL.Insert(new Tables.Log(true) { Message = i + @"123
456" });
#endif
            })))).Start();
        }

        Browser<Tables.Log, RichTextBox> log = null;

        private void لاگToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm(ref log);
        }

        About about = null;

        private void دربارهToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm(ref about);
        }

        Browser<Tables.Patient, ListView> patients = null;

        private void button8_Click(object sender, EventArgs e)
        {
            ShowForm(ref patients);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            ;
        }

        Browser<Tables.UserTable, ListView> userTables = null;

        private void button11_Click(object sender, EventArgs e)
        {
            if (userTables == null || userTables.IsDisposed)
            {
                userTables = new Browser<Tables.UserTable, ListView>();

                userTables.AfterAdd += AddButtons;
                userTables.AfterDelete += AddButtons;

                Func<int, bool> exists = id =>
                null != DB.DataContext.UserTableAssemblys.Where(_ => _.UserTableId == id).FirstOrDefault();

                userTables.BeforeEdit += ut =>
                {
                    if (exists(ut.Id.Value))
                    {
                        MessageBox.Show("این فرم ساخته شده است و امکان ویرایش آن وجود ندارد.‏", Program.Title);

                        return false;
                    }
                    else return true;
                };

                userTables.BeforeDelete += ut =>
                {
                    if (exists(ut.Id.Value))
                    {
                        MessageBox.Show("این فرم ساخته شده است و امکان حذف آن وجود ندارد.‏", Program.Title);

                        return false;
                    }
                    else return true;
                };
            }

            ShowForm(ref userTables);
        }

        Browser<Tables.Column, ListView> columns = null;

        private void button12_Click(object sender, EventArgs e)
        {
            if (columns == null || columns.IsDisposed)
            {
                columns = new Browser<Tables.Column, ListView>();

                Func<int, bool> exists = id =>
                null != DB.DataContext.UserTableAssemblys.Where(_ => _.UserTableId == id).FirstOrDefault();

                columns.BeforeEdit += c =>
                {
                    if (exists(c.UserTableId.Value))
                    {
                        MessageBox.Show("این فیلد متعلق به فرمی ساخته شده است و امکان ویرایش آن وجود ندارد.‏", Program.Title);

                        return false;
                    }
                    else return true;
                };

                columns.BeforeDelete += c =>
                {
                    if (exists(c.UserTableId.Value))
                    {
                        MessageBox.Show("این فیلد متعلق به فرمی ساخته شده است و امکان حذف آن وجود ندارد.‏", Program.Title);

                        return false;
                    }
                    else return true;
                };

                columns.BeforeAdd += c =>
                {
                    if (exists(c.UserTableId.Value))
                    {
                        MessageBox.Show("فرم انتخاب شده ساخته شده است و امکان اضافه کردن فیلد به آن وجود ندارد.‏", Program.Title);

                        return false;
                    }
                    else return true;
                };
            }

            ShowForm(ref columns);
        }


        //صندوق خانواده نوبت ها



    }

}


/*
            var db = new Raven.Client.Embedded.EmbeddableDocumentStore();
            //db.Url = "http://localhost:8080/";
            //db.UseEmbeddedHttpServer = true;
            db.Initialize();
            using (var session = db.OpenSession()) {  }

            var db = new Raven.Server.RavenDbServer();
            //db.Url = "http://localhost:8080/";
            db.UseEmbeddedHttpServer = true;
            db.Initialize();        

            var documentStore = new Raven.Client.Document.DocumentStore()
            { Url = "http://localhost:8080/" };
            documentStore.Initialize();

            using (var session = documentStore.OpenSession())  {  }

            var cs = new System.Data.SQLite.SQLiteConnectionStringBuilder();
            cs.DataSource = "db.sqlite";
            var db = new System.Data.SQLite.SQLiteConnection(cs.ConnectionString).OpenAndReturn();

            //var cs = new FirebirdSql.Data.FirebirdClient.FbConnectionStringBuilder();
            //cs.DataSource = "localhost";
            //cs.Database = "employee.fdb";
            //cs.UserID = "SYSDBA";
            //cs.Password = "masterkey";
            //cs.Charset = "NONE";
            //cs.Pooling = false;
            //cs.ServerType = FirebirdSql.Data.FirebirdClient.FbServerType.Embedded;                                                                                                                                                                                                          

            //if (!System.IO.File.Exists(cs.Database))
            //    FirebirdSql.Data.FirebirdClient.FbConnection.CreateDatabase(
            //        //@"User=SYSDBA;Password=masterkey;Database=SampleDatabase.fdb;DataSource=localhost;Port=3050;Dialect=3;Charset=NONE;Role=;Connection lifetime=15;Pooling=true;MinPoolSize=0;MaxPoolSize=50;Packet Size=8192;ServerType=1;");
            //        cs.ToString());

            //var fb = new FirebirdSql.Data.EntityFramework6.FbConnectionFactory().CreateConnection(
            //    //@"User=SYSDBA;Password=masterkey;Database=SampleDatabase.fdb;DataSource=localhost;Port=3050;Dialect=3;Charset=NONE;Role=;Connection lifetime=15;Pooling=true;MinPoolSize=0;MaxPoolSize=50;Packet Size=8192;ServerType=1;");
            //    cs.ToString());

            //var f = new FirebirdEmbededDbContext(cs.ToString());
            //FirebirdSql.Data.FirebirdClient.FbConnection connection = new FirebirdSql.Data.FirebirdClient.FbConnection(cs.ToString());
*/
