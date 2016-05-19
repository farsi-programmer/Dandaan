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

            textBrowser1.TextFunc = () =>
            {
                var sb = new StringBuilder();
                //var str = "";

                foreach (var item in Tables.Log.Select(textBrowser1.browserMenu1.Page,
                    textBrowser1.browserMenu1.PageSize))
                {
                    // this is if we want to undo, but we don't, we have it in db
                    //str = Regex.Replace(item.Message, Regex.Escape(@"\r\n"), @"\\r\\n");
                    //sb.Append($"{item.Id}\t{item.DateTime}\t{Regex.Replace(str, "\r\n", @"\r\n")}\r\n");

                    // this is specially necessary if we want to get the id for edit or delete
                    sb.Append($"{item.Id}\t{item.DateTime}\t{Regex.Replace(item.Message, "[\r\n]+", " ")}\r\n\r\n");
                }

                return sb.ToString();
            };
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
