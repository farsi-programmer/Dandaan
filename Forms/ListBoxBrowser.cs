﻿using System;
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
    public partial class ListBoxBrowser : ListBoxBrowserBase
    {
        public ListBoxBrowser()
        {
            InitializeComponent();

            browserMenu1.Act = Act(listBox1);
        }
    }
}
