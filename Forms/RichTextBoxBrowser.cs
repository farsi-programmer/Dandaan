using System;
using System.Text.RegularExpressions;
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
    public partial class RichTextBoxBrowser<T> : TextBoxBrowserBase<T> where T : class
    {
        public RichTextBoxBrowser()
        {
            InitializeComponent();

            View = richTextBox1;
        }

#if using_ef || using_sqlite
        public static void Log(string message)
        {
            DB.Run((context) =>
            {
                context.Logs.Add(new Log() { Message = message });
                context.SaveChanges();
            });
    }
#endif
    }
}
