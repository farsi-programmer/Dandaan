// http://offtopic.blog.ir/

using System;
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

        RichTextBoxBrowser<Tables.Log> log = null;

        private void لاگToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showForm(ref log);
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
            ;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var f = new Form() { AutoScroll = true };

            var ps = typeof(Tables.Patient).GetProperties();

            int y = 10, x = ClientSize.Width, z = 350, m = 2;

            using (var g = CreateGraphics())
                foreach (var item in ps)
                {
                    var da = Reflection.GetDandaanAttribute(item);

                    if (item.PropertyType == typeof(string))
                    {
                        var label = da.Label + ":";

                        var s = g.MeasureString(label, Font).ToSize();

                        var l = new Label()
                        {
                            Text = label,
                            TextAlign = ContentAlignment.TopRight,
                            //BorderStyle = BorderStyle.FixedSingle,
                            Margin = new Padding(0),
                            Padding = new Padding(0),
                            Size = new Size(s.Width + 5, s.Height),
                            RightToLeft = RightToLeft.Yes,
                        };

                        l.Location = new Point(ClientSize.Width - l.Size.Width, y);

                        if (x > l.Location.X) x = l.Location.X;

                        var tb = new TextBox()
                        {
                            Width = z,
                            Margin = new Padding(0),
                            Padding = new Padding(0),
                            Location = new Point(x - z - m, y),
                            RightToLeft = RightToLeft.Yes,
                        };

                        if (l.Height > tb.Height) tb.Location = new Point(tb.Location.X, tb.Location.Y
                            + (l.Height - tb.Height) / 2);
                        else if (tb.Height > l.Height) l.Location = new Point(l.Location.X, l.Location.Y
                            + (tb.Height - l.Height) / 2);

                        f.Controls.Add(l);
                        f.Controls.Add(tb);
                        y += (l.Height > tb.Height ? l.Height : tb.Height) + 8;
                    }
                }

            foreach (Control item in f.Controls)
                if (item is Label) { if (item.Location.X > x) item.Location = new Point(x, item.Location.Y); }
                else if (item.Location.X > x - z - m) item.Location = new Point(x - z - m, item.Location.Y);

            f.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        ListViewBrowser<Tables.Patient> patients = null;

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

        private void button10_Click(object sender, EventArgs e)
        {
            var rtb = new RichTextBox() { Location = new Point(Width, Height) };
            rtb.SuspendLayout();
            Controls.Add(rtb);
            MessageBox.Show(rtb.Rtf);

            MessageBox.Show((new RichTextBox() { Text = "" }.Rtf == richTextBox1.Rtf).ToString());
            MessageBox.Show(new RichTextBox() { Text = "" }.Rtf);
            MessageBox.Show(richTextBox1.Rtf);
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
