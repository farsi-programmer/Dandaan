using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dandaan.Forms
{
    public partial class Browser : UserControl
    {
        public Browser()
        {
            Disposed += Browser_Disposed; ;

            InitializeComponent();
        }

        private void Browser_Disposed(object sender, EventArgs e)
        {
            thread?.Abort();
        }

        protected System.Threading.Thread thread = null;
    }
}
