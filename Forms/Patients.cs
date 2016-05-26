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

            /*listBrowser1.browserMenu1.CountFunc = SQL.Count<Tables.Patient>;

            listBrowser1.ObjsFunc = () =>
                SQL.Select<Tables.Patient>(listBrowser1.browserMenu1.Page,
                    listBrowser1.browserMenu1.PageSize).Select((patient) =>
                    $"{patient.PatNum}\t{Regex.Replace(patient.LName + "\t" + patient.FName, "[\r\n]+", " ")}");*/

            listBrowser1.browserMenu1.CountFunc = SQL.Count<Tables.Log>;

            using (var g = CreateGraphics())
            {
                SizeF size;
                var ps = typeof(Tables.Patient).GetProperties();
                foreach (var item in ps)
                {
                    var label = Reflection.GetDandaanAttribute(item).Label;

                    size = g.MeasureString(label, Font);

                    listBrowser1.listView1.Columns.Add(label).Width = (int)size.Width + 10;
                }
            }

            bool odd = true;
            listBrowser1.ItemsFunc = () =>
                SQL.Select<Tables.Log>(listBrowser1.browserMenu1.Page,
                    listBrowser1.browserMenu1.PageSize).Select((patient) =>
                    {
                        odd = !odd;
                        return new ListViewItem(new string[]
                        {
                            $"{patient.Id}",
                            $"{Regex.Replace(patient.Message, "[\r\n]+", " ")}"
                        })
                        { BackColor = odd ? Color.LightCyan : Color.FromArgb(0xcf, 0xff, 0xcf) };
                    });
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
