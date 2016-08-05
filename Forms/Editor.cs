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
    public partial class Editor<T> : Form where T : class
    {
        public Editor()
        {
            InitializeComponent();
        }

        public Editor(string text) : this() // this is called first
        {
            Text = text;
        }

        public void setEditor(UserControls.Editor<T> editor)
        {
            Controls.Add(editor);

            ClientSize = editor.ClientSize;

            if (Width > 800) Width = 800;
            if (Height > 600) Height = 600;

            FormClosing += (_, __) =>
            {
                foreach (Control c in editor.Controls)
                    if (c.BackColor == UserControls.Editor<object>.EditColor)
                    {
                        if (MessageBox.Show("آیا مطمئن هستید؟", Text, MessageBoxButtons.YesNo) == DialogResult.No)
                            __.Cancel = true;

                        break;
                    }
            };
        }
    }
}
