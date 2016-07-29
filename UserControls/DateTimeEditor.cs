using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dandaan.UserControls
{
    public partial class DateTimeEditor : UserControl
    {
        public DateTimeEditor()
        {
            InitializeComponent();
        }

        public DateTimeEditor(bool iranian):this()
        {
            foreach (var item in Controls)
            {
                if (item is ComboBox)
                {
                    (item as ComboBox).AutoCompleteSource = AutoCompleteSource.ListItems;
                    (item as ComboBox).AutoCompleteMode = AutoCompleteMode.Append;
                }
            }

            for (int i = 0; i < 60; i++) comboBoxSecond.Items.Add(i);

            for (int i = 0; i < 60; i++) comboBoxMinute.Items.Add(i);

            for (int i = 0; i < 24; i++) comboBoxHour.Items.Add(i);

            for (int i = 1; i < 32; i++) comboBoxDay.Items.Add(i);

            for (int i = 1; i < 13; i++) comboBoxMonth.Items.Add(i);

            if(iranian) for (int i = 1300; i < 1400; i++) comboBoxYear.Items.Add(i);
            else for (int i = 1920; i < 2020; i++) comboBoxYear.Items.Add(i);
        }
    }
}
