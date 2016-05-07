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
    public partial class Patients : Form
    {
        public Patients()
        {
            InitializeComponent();
        }

        private void Patients_Resize(object sender, EventArgs e)
        {
            if (lastFormWindowState != WindowState && WindowState != FormWindowState.Minimized)
                lastFormWindowState = WindowState;
        }

        private void Patients_ResizeBegin(object sender, EventArgs e)
        {
            ;
        }

        private void Patients_SizeChanged(object sender, EventArgs e)
        {
            ;
        }
    }
}
