﻿using System;
using System.Text.RegularExpressions;
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
    public partial class TextBoxBrowser : TextBoxBrowserBase
    {
        public TextBoxBrowser()
        {
            InitializeComponent();

            browserMenu1.ChangeFocus = textBox1.Focus;

            browserMenu1.Act = Act(textBox1);
        }
    }
}