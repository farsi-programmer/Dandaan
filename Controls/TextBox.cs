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
    public partial class TextBox : System.Windows.Forms.TextBox
    {
        public TextBox()
        {
            InitializeComponent();

            DefaultBackColor_ = BackColor;
        }

        public string DefaultText { get; set; } = "";

        public Color DefaultBackColor_ { get; }

        protected override void OnPaint(PaintEventArgs pe) => base.OnPaint(pe);

        public void RaiseTextChanged() => OnTextChanged(new EventArgs());
    }
}
