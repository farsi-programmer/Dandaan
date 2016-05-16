﻿using System;
using System.Text.RegularExpressions;
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
    public partial class TextBrowser : UserControl
    {
        public Func<int, int, string> TextFunc; // can be null, because count is zero

        public TextBrowser()
        {
            InitializeComponent();

            browser1.Act = (count, page, pageSize, setWorking) =>
            {
                if (count == 0)
                {
                    textBox1.Text = "There are no records.";
                    setWorking(false);
                }
                else
                {
                    var selection = textBox1.SelectedText;
                    var selectionStart = textBox1.SelectionStart;
                    textBox1.Text = "Please wait...";

                    new System.Threading.Thread(() =>
                    {
#if using_ef || using_sqlite
            DB.Run((context) =>
            {
                foreach (var item in context.Logs)
                {
                    textBox1.AppendText(item.Id + "\t" + item.DateTime + "\t" + item.Message + "\r\n");
                }
            });
#else
                        Invoke(new Action(() =>
                        {
                            textBox1.Text = TextFunc(page, pageSize);

                            if (selection.Length == 0 && selectionStart < textBox1.Text.Length)
                                textBox1.SelectionStart = selectionStart;
                            else
                            {
                                var i = textBox1.Text.IndexOf(selection);
                                if (i > -1) textBox1.Select(i, selection.Length + i < textBox1.Text.Length ? selection.Length : textBox1.Text.Length - i - 1);
                            }

                            textBox1.ScrollToCaret();
                            setWorking(false);
                        }));
                    }).Start();
                }
#endif
            };
        }

        public void Close() => browser1.Close();

        public void SetCountFunc(Func<int> countFunc) => browser1.CountFunc = countFunc;
    }
}