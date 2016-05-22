using System;
using System.Text.RegularExpressions;
using System.Data.Entity;
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
    public partial class Logger : Form
    {
        public Logger()
        {
            InitializeComponent();

            textBrowser1.browserMenu1.CountFunc = SQL.Count<Tables.Log>;

            textBrowser1.LinesFunc = () =>
                Tables.Log.Select(textBrowser1.browserMenu1.Page,
                    textBrowser1.browserMenu1.PageSize).Select((log) =>
                    // replacing new lines is specially necessary if we want to get the id for edit or delete
                    $"{log.Id}\t{log.DateTime}\t{Regex.Replace(log.Message, "[\r\n]+", " ")}");

            // this is if we want to undo, but we don't, we have it in db
            //str = Regex.Replace(log.Message, Regex.Escape(@"\r\n"), @"\\r\\n");
            //sb.Append($"{log.Id}\t{log.DateTime}\t{Regex.Replace(str, "\r\n", @"\r\n")}\r\n");
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
