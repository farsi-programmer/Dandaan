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
    public partial class TextBoxBrowser<T> : TextBoxBrowserBase<T> where T : class
    {
        public TextBoxBrowser()
        {
            InitializeComponent();

            View = textBox1;
        }
    }
}
