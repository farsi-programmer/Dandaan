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
    [Table(Name = nameof(Patient))]
    [Dandaan(Label = "بیماران", EnableAdd = true, EnableDelete = true, EnableEdit = true, EnableSearch = true)]
    public class Patient
    {
        [Column(IsPrimaryKey = true)]
        [DandaanColumn(Sql = "[int] IDENTITY NOT NULL CONSTRAINT [PK_" + nameof(Patient) + @"]
PRIMARY KEY CLUSTERED",
            Label = "شماره بیمار")]
        /// <summary>PatNum.</summary>
        public int? PatNum { get; set; } // i don't want to make SSN mandatory, so this is the primary key

        [Column]
        [DandaanColumn(Sql = "[nvarchar](100) NOT NULL", Label = "نام خانوادگی")]
        /// <summary>Last name.</summary>
        public string LName { get; set; }

        [Column]
        [DandaanColumn(Sql = "[nvarchar](100) NOT NULL",// we don't use defaults, it makes things complicated  CONSTRAINT [DF_" + nameof(Patient) + "_" + nameof(FName) + "] DEFAULT (N'')",
            Label = "نام")]
        /// <summary>First name.</summary>
        public string FName { get; set; }// = "";

        [Column]
        [DandaanColumn(Sql = "[nvarchar](100) NOT NULL",// CONSTRAINT [DF_" + nameof(Patient) + "_" + nameof(HmPhone) + "] DEFAULT (N'')",
            Label = "تلفن منزل")]
        /// <summary>Home phone. Includes any punctuation</summary>
        public string HmPhone { get; set; }// = "";

        [Column]
        [DandaanColumn(Sql = "[nvarchar](100) NOT NULL",// CONSTRAINT [DF_" + nameof(Patient) + "_" + nameof(WirelessPhone) + "] DEFAULT (N'')",
            Label = "شماره موبایل")]
        /// <summary>.</summary>
        public string WirelessPhone { get; set; }// = "";

        [Obsolete("For Linq2Sql.", true)]
        public Patient() { }

        public Patient(bool withDefaultValuesForInsert)
        {
            if (withDefaultValuesForInsert)
            {
                WirelessPhone = "";
                HmPhone = "";
                FName = "";
            }
        }
    }
}
