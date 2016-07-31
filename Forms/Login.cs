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
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();

            AcceptButton = button1;
            CancelButton = button2;            
        }        

        private void button1_Click(object sender, EventArgs e)
        {
            var id = Tables.User.Login(textBox1.Text, textBox2.Text);

            if (id > 0)
            {
                if (Tables.User.IsEnabled(id))
                {
                    Program.UserId = id;
                    Program.LocalSettings.LoginUserName = textBox1.Text;
                    Program.WriteLocalSettings();

                    DialogResult = DialogResult.OK;
                }
                else
                {
                    textBox3.BackColor = Color.PaleVioletRed;
                    textBox3.AppendText("نام کاربری غیر فعال شده است.\r\n");
                }
            }
            else
            {
                textBox3.BackColor = Color.PaleVioletRed;
                textBox3.AppendText("نام کاربری و یا پسورد اشتباه وارد شده است.\r\n");
            }
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            var c = SQL.Count<Tables.User>();

            if (c < 1)
            {
                textBox3.AppendText("شما اولین کاربر برنامه هستید، بنابراین میتوانید با نام کاربری"
+" admin و بدون وارد کردن پسورد به برنامه وارد شوید.\r\n");
                textBox1.Text = "admin";

                Tables.User.Add(new Tables.User(true) { Name = "admin" });
            }
            else if (c == 1)
            {
                textBox3.AppendText("برنامه فقط یک کاربر دارد.\r\n");
                textBox1.Text = "admin";
            }
            else
            {
                textBox1.Text = Program.LocalSettings.LoginUserName;
            }

            textBox1.Focus();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
