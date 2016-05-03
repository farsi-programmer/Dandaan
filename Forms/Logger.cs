using System;
using System.Data.Entity;
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
    public partial class Logger : Form
    {
        public Logger()
        {
            InitializeComponent();
        }

#if using_ef || using_sqlite
        public static void Log(string message)
        {
            DB.Run((context) =>
            {
                context.Logs.Add(new Log() { Message = message });
                context.SaveChanges();
            });
    }
#endif

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();

#if using_ef || using_sqlite
            DB.Run((context) =>
            {
                foreach (var item in context.Logs)
                {
                    textBox1.AppendText(item.Id + "\t" + item.DateTime + "\t" + item.Message + "\r\n");
                }
            });
#else
            foreach(var item in Tables.Log.Select())
            {
                textBox1.AppendText(item.Id + "\t" + item.DateTime + "\t" + item.Message + "\r\n");
            }
#endif
        }

        private void FormLogger_Load(object sender, EventArgs e)
        {
            button1_Click(null, null);
        }
    }



}
