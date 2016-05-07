using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dandaan.Forms
{
    // a recompile is needed for changes to this class to take effect for the designer
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
            Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));

            Text = "مدیریت دندانپزشکی";
        }
    }
}
