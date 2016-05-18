// http://offtopic.blog.ir/

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
        }

        private void Main_SizeChanged(object sender, EventArgs e)
        {
            ;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            ;
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

        Logger logger = null;

        private void لاگToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showForm(ref logger);
        }

        About about = null;

        private void دربارهToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showForm(ref about);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SQL.Insert(new Tables.Log()
            {
                Message =
                "00000000000000000000000000000000000000000000000000000000000000"
                + "00000000000000000000000000000000000000000000000000000000000000"
                + "00000000000000000000000000000000000000000000000000000000000000"
                + "00000000000000000000000000000000000000000000000000000000000000"
                + "00000000000000000000000000000000000000000000000000000000000000"
                + "00000000000000000000000000000000000000000000000000000000000000"
                + "00000000000000000000000000000000000000000000000000000000000000"
                + "00000000000000000000000000000000000000000000000000000000000000"
                + "00000000000000000000000000000000000000000000000000000000000000"
                + "00000000000000000000000000000000000000000000000000000000000000"
                + "00000000000000000000000000000000000000000000000000000000000000000"
            });
        }

        private void Main_Shown(object sender, EventArgs e)
        {
            ;
        }

        private void Main_VisibleChanged(object sender, EventArgs e)
        {
            ;
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Dandaan.DB.Connection.Database);
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        Patients patients = null;

        private void button8_Click(object sender, EventArgs e)
        {
            showForm(ref patients);
        }

        void showForm<T>(ref T f) where T : Form
        {
            if (f == null || f.IsDisposed) f = Activator.CreateInstance<T>();

            if (f.Visible == true)
            {
                if (f.WindowState == FormWindowState.Minimized)
                    f.WindowState = f.lastFormWindowState;
                else f.Focus();//BringToFront();
            }
            else f.Show();//(this); this keeps the form on top, which i don't like
        }

        private void button9_Click(object sender, EventArgs e)
        {
            throw new Exception("");
        }
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
