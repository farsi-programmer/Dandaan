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
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            ;
        }

        private void خروجToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Parallel.For(0, 10, new Action<int>((i) =>
            {
                DB.Run((c) => FormLogger.Log("تست " + c.Logs.Count()));
            }));
        }

        private void لاگToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormLogger().Show();
        }

        private void دربارهToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormAbout().Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ;
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            ;
        }

        private void FormMain_VisibleChanged(object sender, EventArgs e)
        {
            ;
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
