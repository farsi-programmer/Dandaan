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
    public partial class ListBoxBrowser<T> : ListBoxBrowserBase<T> where T : class
    {
        public ListBoxBrowser()
        {
            InitializeComponent();

            ListBox = listBox1;
        }
    }
}
