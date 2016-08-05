using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Data.Linq.Mapping;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dandaan.Tables
{
    [Table(Name = nameof(Appointment))]
    [Dandaan(Label = "نوبت​ها", EnableAdd = true, EnableDelete = true, EnableEdit = true, EnableSearch = true)]
    public class Appointment
    {
        [Column(IsPrimaryKey = true)]
        [DandaanColumn(Sql = "[int] IDENTITY NOT NULL CONSTRAINT [PK_" + nameof(Appointment) + @"]
PRIMARY KEY CLUSTERED",
            Label = "شماره نوبت")]
        public int? AptNum { get; set; }

        [DandaanColumn(Sql = "[int] NOT NULL CONSTRAINT [FK_" + nameof(Appointment) + "_" + nameof(Patient) + @"]
FOREIGN KEY REFERENCES [dbo].[" + nameof(Patient) + "] ([" + nameof(Patient.PatNum) + "])",
            Label = "شماره بیمار")]
        public int? PatNum { get; set; }

        ///<summary>Note.</summary>
        public string Note;

//        ///<summary>FK to provider.ProvNum.</summary>
//        public long ProvNum;

        public DateTime AptDateTime;

        //        ///<summary>FK to insplan.PlanNum for the primary insurance plan at the time the appointment is set complete. May be 0. We can't tell later which subscriber is involved; only the plan.</summary>
        //        public long InsPlan1;

        [Obsolete("For Linq2Sql.", true)]
        public Appointment() { }

        public Appointment(bool withDefaultValuesForInsert)
        {
            if (withDefaultValuesForInsert)
            {
                ;
            }
        }
    }
}









