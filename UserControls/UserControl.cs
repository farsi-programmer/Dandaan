﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dandaan.UserControls
{
    public partial class UserControl : System.Windows.Forms.UserControl
    {
        public UserControl()
        {
            InitializeComponent();
        }

        public object Invoke(Action act) => base.Invoke(act);
    }
}
