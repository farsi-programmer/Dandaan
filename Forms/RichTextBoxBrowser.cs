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
    public partial class RichTextBoxBrowser<T> : Browser<T> where T : class
    {
        public RichTextBoxBrowser()
        {
            InitializeComponent();

            Init(richTextBoxBrowser1.browserMenu1);

            richTextBoxBrowser1.LinesFunc = () =>
            SQL.Select<T>(richTextBoxBrowser1.browserMenu1.Page, richTextBoxBrowser1.browserMenu1.PageSize)
            .Select((row) =>
            {
                var sb = new StringBuilder();
                foreach (var item in propertyInfos)
                {
                    var obj = item.GetValue(row);
                    
                    // replacing new lines is specially necessary if we want to get the id for edit or delete
                    if (obj is string) obj = Regex.Replace(obj as string, "[\r\n]+", " ");

                    sb.Append(obj + "\t");
                }
                return sb.ToString();
            });

            // this is if we want to be able to undo, but we don't, we have it in db
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
