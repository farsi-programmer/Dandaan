// http://offtopic.blog.ir/

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
            var s = Tables.Setting.SelectOrInsertDefault(Program.UserId);
            if (s.MainFormWindowState != FormWindowState.Minimized)
                WindowState = s.MainFormWindowState;
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
            var s = Tables.Setting.SelectOrInsertDefault(Program.UserId);
            s.MainFormWindowState = WindowState;
            Tables.Setting.Update(s);
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
        Dictionary<string, Assembly> assemblies;

        public void AddButtons()
        { 
            int margin = 6, left = ClientSize.Width, bottom = ClientSize.Height,
                w = buttonPatients.Width, h = buttonPatients.Height;

            Action<Control> act = (item) =>
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

            var tables = SQL.SelectAll<Tables.UserTable>();

            foreach (var item in tables)
            {
                Button button = new Button()
                {
                    Text = item.Label,
                    Location = new Point(left, bottom),
                    Anchor = buttonPatients.Anchor,
                    Width = w,
                    Height = h,
                };

                button.Click += (_, __) =>
                {
                    var name = nameof(Dandaan) + "." + nameof(Tables) + "." + nameof(Tables.UserTable) + item.Id;
                    var path = Program.DataDirectory + "\\" + name + ".dll";
                    var contextName = nameof(DataContext) + item.Id;

                    Action B = () =>
                    {
                        if (assemblies == null) assemblies = new Dictionary<string, Assembly>();

                        Assembly assembly;

                        if (assemblies.ContainsKey(path)) assembly = assemblies[path];
                        else
                        {
                            assembly = Assembly.LoadFile(path);
                            assemblies.Add(path, assembly);
                        }

                        Type t = assembly.GetType(name);

                        Form form = null;

                        Action C = () =>
                        {
                            form = (Form)Activator.CreateInstance(
                                typeof(ListViewBrowser<>).MakeGenericType(new[] { t }));
                        };

                        if (userForms == null) userForms = new Dictionary<string, Form>();

                        if (userForms.ContainsKey(button.Text))
                        {
                            form = userForms[button.Text];

                            if (form.IsDisposed) C();
                        }
                        else
                        {
                            DB.DataContextType = assembly.GetType(nameof(Dandaan) + "." + contextName);

                            SQL.CreateTable(t);

                            C();

                            userForms.Add(button.Text, form);
                        }

                        ShowForm(ref form);
                    };

                    Func<string, bool> exists = (p) =>
                    {
                        if (!File.Exists(p) || new FileInfo(p).Length == 0)
                        {
                            var row = SQL.SelectFirstWithWhere(new Tables.UserTableAssembly()
                            { UserTableId = item.Id }, false);

                            if (row != null) File.WriteAllBytes(p, row.Assembly);
                            else return false;
                        }

                        return true;
                    };

                    if (exists(path)) B();
                    else
                    {
                        var unbuiltReferences = false;
                        var unbuiltName = "";
                        var columns = FormBuilder.GetColumns(item);

                        foreach (var A in columns)
                            if (A.ReferenceColumnId != null)
                            {
                                var n = nameof(Dandaan) + "." + nameof(Tables) + "." + nameof(Tables.UserTable) + A.ReferenceColumnId.Value;
                                var p = Program.DataDirectory + "\\" + n + ".dll";

                                if (!exists(p))
                                {
                                    unbuiltReferences = true;

                                    unbuiltName = SQL.SelectFirstWithWhere(new Tables.UserTable()
                                    {
                                        Id = SQL.SelectFirstWithWhere(new Tables.Column(false)
                                        { Id = A.ReferenceColumnId.Value }, false).UserTableId
                                    }, false).Label;

                                    break;
                                }
                            }

                        if (unbuiltReferences) MessageBox.Show($"این فرم به فرم:‏\r\n{unbuiltName}\r\nارجاع داده است، لطفا ابتدا آنرا بسازید.‏", Program.Title);
                        else if (MessageBox.Show("این فرم هنوز ساخته نشده است، در صورت ساختن آن امکان تغییر دادن این فرم یا فیلدهای آن دیگر وجود نخواهد داشت!‏",
                     Program.Title, MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            new FormBuilder(path, name, item);
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
                SQL.Insert(new Tables.Log() { Message = i + @"123
456" });
#endif
            })))).Start();
        }

        RichTextBoxBrowser<Tables.Log> log = null;

        private void لاگToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm(ref log);
        }

        About about = null;

        private void دربارهToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm(ref about);
        }

        ListViewBrowser<Tables.Patient> patients = null;

        private void button8_Click(object sender, EventArgs e)
        {
            ShowForm(ref patients);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            ;
        }

        ListViewBrowser<Tables.UserTable> userTables = null;

        private void button11_Click(object sender, EventArgs e)
        {
            if (userTables == null || userTables.IsDisposed)
            {
                userTables = new ListViewBrowser<Tables.UserTable>();

                userTables.AfterAdd += AddButtons;
                userTables.AfterDelete += AddButtons;

                Func<int, bool> exists = (id) =>
                {
                    var row = SQL.SelectFirstWithWhere(new Tables.UserTableAssembly()
                    { UserTableId = id }, false);

                    return row != null;
                };

                userTables.BeforeEdit += (o) =>
                {
                    if (exists(o.Id.Value))
                    {
                        MessageBox.Show("این فرم ساخته شده است و امکان ویرایش آن وجود ندارد.‏", Program.Title);

                        return false;
                    }
                    else return true;
                };

                userTables.BeforeDelete += (o) =>
                {
                    if (exists(o.Id.Value))
                    {
                        MessageBox.Show("این فرم ساخته شده است و امکان حذف آن وجود ندارد.‏", Program.Title);

                        return false;
                    }
                    else return true;
                };
            }

            ShowForm(ref userTables);
        }

        ListViewBrowser<Tables.Column> columns = null;

        private void button12_Click(object sender, EventArgs e)
        {
            if (columns == null || columns.IsDisposed)
            {
                columns = new ListViewBrowser<Tables.Column>();

                Func<int, bool> exists = (id) =>
                {
                    var row = SQL.SelectFirstWithWhere(new Tables.UserTableAssembly()
                    { UserTableId = id }, false);

                    return row != null;
                };

                columns.BeforeEdit += (o) =>
                {
                    if (exists(o.UserTableId.Value))
                    {
                        MessageBox.Show("این فیلد متعلق به فرمی ساخته شده است و امکان ویرایش آن وجود ندارد.‏", Program.Title);

                        return false;
                    }
                    else return true;
                };

                columns.BeforeDelete += (o) =>
                {
                    if (exists(o.UserTableId.Value))
                    {
                        MessageBox.Show("این فیلد متعلق به فرمی ساخته شده است و امکان حذف آن وجود ندارد.‏", Program.Title);

                        return false;
                    }
                    else return true;
                };

                columns.BeforeAdd += (o) =>
                {
                    if (exists(o.UserTableId.Value))
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
