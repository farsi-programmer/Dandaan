using System;
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
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();

            CommonFormStuff.DoCommonSettings(this);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            var c = Tables.DandaanUser.Count();

            if (c < 1)
            {
                textBox3.AppendText(@"شما اولین کاربر برنامه هستید"
+ @"، بنابراین میتوانید با نام کاربری admin و"
+ @" بدون وارد کردن پسورد به برنامه وارد شوید.");

                textBox1.Text = "admin";

                Tables.DandaanUser.Insert(new Tables.DandaanUser() { Name = "admin" });
            }
            else if (c == 1)
            {
                textBox3.AppendText(@"شما تنها کاربر برنامه هستید.");

                textBox1.Text = "admin";
            }
            //else
        }
    }
}
