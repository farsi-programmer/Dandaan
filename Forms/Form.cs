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
    // a recompile is needed for changes in here to take effect in the designer  

    public partial class Form : System.Windows.Forms.Form
    {
        protected FormWindowState lastFormWindowState;

        public Form()
        {
            InitializeComponent();

            StartPosition = FormStartPosition.CenterScreen;
        }

        private void Form_Resize(object sender, EventArgs e)
        {
            if (lastFormWindowState != WindowState && WindowState != FormWindowState.Minimized)
                lastFormWindowState = WindowState;
        }

        protected object Invoke(Action act) => base.Invoke(act);

        List<Form> openForms = new List<Form>();

        protected void ShowForm<T>(ref T f) where T : Forms.Form
        {
            if (f == null || f.IsDisposed) f = Activator.CreateInstance<T>();

            if (!openForms.Contains(f)) openForms.Add(f);

            if (f.Visible == true)
            {
                if (f.WindowState == FormWindowState.Minimized)
                    f.WindowState = f.lastFormWindowState;
                else f.Select();//Focus();//BringToFront();
            }
            else f.Show();//(this); this keeps the form on top, which i don't like
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (var item in openForms) if (item != null) item.Close();
        }
    }
}


