using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dandaan.Controls
{
    public partial class ButtonEdit : System.Windows.Forms.Button
    {
        public ButtonEdit()
        {
            InitializeComponent();

            _DefaultBackColor = BackColor;
        }

        public string DefaultText { get; set; } = "";

        public Color _DefaultBackColor { get; set; }

        public object Obj { get; set; }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        public void RaiseTextChanged()
        {
            OnTextChanged(new EventArgs());
        }
    }
}
