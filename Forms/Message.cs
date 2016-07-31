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
    public partial class Message : Form
    {
        public Message()
        {
            InitializeComponent();

            CancelButton = button1;
        }

        public Message(string title, string message) : this()
        {
            Text = title;
            textBox1.Text = message;
            button1.Select();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
