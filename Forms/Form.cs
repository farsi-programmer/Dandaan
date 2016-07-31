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

            AutoScroll = true;

            StartPosition = FormStartPosition.CenterScreen;
        }

        private void Form_Resize(object sender, EventArgs e)
        {
            if (lastFormWindowState != WindowState && WindowState != FormWindowState.Minimized)
                lastFormWindowState = WindowState;
        }

        protected object Invoke(Action act) => base.Invoke(act);

        List<Form> formsToClose = new List<Form>();

        protected void ShowForm<T>(ref T f, bool shouldClose = true) where T : Form, new()
        {
            if (f == null || f.IsDisposed) f = new T();//Activator.CreateInstance<T>();

            if (shouldClose && !formsToClose.Contains(f)) formsToClose.Add(f);

            if (f.Visible == true)
            {
                if (f.WindowState == FormWindowState.Minimized)
                    f.WindowState = f.lastFormWindowState;
                else f.Activate();//Select();//Focus();//BringToFront();
            }
            else f.Show();//(this); this keeps the form on top, which i don't like
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (var item in formsToClose) item?.Close();
        }
    }
}


