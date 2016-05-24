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
    public partial class Patients : Form
    {
        public Patients()
        {
            InitializeComponent();

            listBrowser1.browserMenu1.CountFunc = SQL.Count<Tables.Log>;

            listBrowser1.ObjsFunc = () =>
                Tables.Log.Select(listBrowser1.browserMenu1.Page,
                    listBrowser1.browserMenu1.PageSize).Select((log) =>
                    // replacing new lines is specially necessary if we want to get the id for edit or delete
                    $"{log.Id}\t{log.DateTime}\t{Regex.Replace(log.Message, "[\r\n]+", " ")}");
        }

        private void Patients_ResizeBegin(object sender, EventArgs e)
        {
            ;
        }

        private void Patients_SizeChanged(object sender, EventArgs e)
        {
            ;
        }

        private void Patients_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
