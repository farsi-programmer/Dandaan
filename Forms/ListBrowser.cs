using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dandaan.Forms
{
    public partial class ListBrowser : UserControl
    {
        public ListBrowser()
        {
            InitializeComponent();

            browserMenu1.Act = (count, page, pageSize, setWorking, pageChange) =>
            {
                if (count == 0)
                {
                    listBox1.Items.Clear();
                    listBox1.Items.Add("رکوردی وجود ندارد.");
                    setWorking(false);
                }
                else
                {
                    /*var selection = textBox1.SelectedText;
                    var selectionStart = textBox1.SelectionStart;*/
                    listBox1.Items.Clear();
                    listBox1.Items.Add("لطفا صبر کنید.");

                    new System.Threading.Thread(() =>
                    {
                        Invoke(new Action(() =>
                        {
                            listBox1.Items.Clear();
                            /*textBox1.Text = TextFunc(page, pageSize);

                            if (selection.Length == 0 && selectionStart < textBox1.Text.Length)
                                textBox1.SelectionStart = selectionStart;
                            else
                            {
                                var i = textBox1.Text.IndexOf(selection);
                                if (i > -1) textBox1.Select(i, selection.Length + i < textBox1.Text.Length ? selection.Length : textBox1.Text.Length - i - 1);
                            }

                            textBox1.ScrollToCaret();*/
                            setWorking(false);
                        }));
                    }).Start();
                }
            };
        }

        public void Close() => browserMenu1.Close();

        public void SetCountFunc(Func<int> countFunc) => browserMenu1.CountFunc = countFunc;
    }
}
