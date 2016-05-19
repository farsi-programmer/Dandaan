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
    public partial class CheckedListBrowser : ListBrowser
    {
        public CheckedListBrowser()
        {
            InitializeComponent();

            browserMenu1.ChangeFocus = checkedListBox1.Focus;

            browserMenu1.Act = Act(checkedListBox1);
        }
    }
}
