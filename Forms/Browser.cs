using System;
using System.Reflection;
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
    public partial class Browser<T> : Form where T : class
    {
        public Browser()
        {
            InitializeComponent();
        }

        protected DandaanAttribute dandaanAttribute;
        protected PropertyInfo[] propertyInfos;

        protected void Init(UserControls.BrowserMenu browserMenu)
        {
            dandaanAttribute = Reflection.GetDandaanAttribute(typeof(T));

            Text = dandaanAttribute.Label;

            propertyInfos = typeof(T).GetProperties();

            browserMenu.DandaanAttribute = dandaanAttribute;
            browserMenu.CountFunc = SQL.Count<T>;
            browserMenu.AddAct = AddAct;
        }

        Editor<T> editor = null;
        protected void AddAct()
        {
            if (editor == null || editor.IsDisposed)
            {
                editor = new Editor<T>(propertyInfos);
                editor.Text = dandaanAttribute.Label;
            }

            Common.showForm(ref editor);
        }
    }
}
