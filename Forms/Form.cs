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
    // a recompile is needed for changes in here to take effect in the designer  

    public partial class Form : System.Windows.Forms.Form
    {
        public FormWindowState lastFormWindowState;

        public Form()
        {
            InitializeComponent();

            StartPosition = FormStartPosition.CenterScreen;

            //AutoScaleMode = AutoScaleMode.Font;

            Font = new Font("Microsoft Sans Serif", 14.25f);

            Text = "مدیریت دندانپزشکی";

            Resize += Form_Resize;
        }

        private void Form_Resize(object sender, EventArgs e)
        {
            if (lastFormWindowState != WindowState && WindowState != FormWindowState.Minimized)
                lastFormWindowState = WindowState;
        }
    }
}


