using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dandaan.Forms
{
    // a recompile is needed for changes in here to take effect for the designer

    public class Form : FormFontText
    {
        public FormWindowState lastFormWindowState;

        public Form()
        {
            StartPosition = FormStartPosition.CenterScreen;
        }
    }

    public class FormFontText : System.Windows.Forms.Form
    {
        public FormFontText()
        {
            AutoScaleMode = AutoScaleMode.Font;

            Font = new Font("Microsoft Sans Serif", 14.25f);

            Text = "مدیریت دندانپزشکی";
        }
    }
}
