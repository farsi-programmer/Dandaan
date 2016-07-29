using System;
using System.Diagnostics;
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
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();

            //var act = (_) => Process.Start(_);

            //linkLabel1.LinkClicked += (_, __) => Process.Start(linkLabel1.Text);

            //linkLabel2.LinkClicked += (_, __) => Process.Start(linkLabel2.Text);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(linkLabel1.Text);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(linkLabel2.Text);
        }
    }
}
