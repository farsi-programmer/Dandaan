using System;
using System.Reflection;
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
    [Table(Name = nameof(Log))]
    [Dandaan(Label = "لاگ​ها", EnableSearch = true)]
    public class Log
    {
        [Column]//(IsPrimaryKey = true, IsDbGenerated = true)]
        [DandaanColumn(Sql = "[int] IDENTITY NOT NULL CONSTRAINT [PK_" + nameof(Log) + @"]
PRIMARY KEY CLUSTERED (Id DESC)",
            Label = "شماره")]
        public int? Id { get; set; }

        [Column]//(IsDbGenerated = true)]
        [DandaanColumn(Sql = "[smalldatetime] NOT NULL",// we don't use defaults, it makes things complicated CONSTRAINT [DF_" + nameof(Log) + "_" + nameof(DateTime) + "] DEFAULT (getdate())",
            Label = "تاریخ")]
        public DateTime? DateTime { get; set; } = System.DateTime.Now;

        [Column]
        [DandaanColumn(Sql = "[nvarchar](1000) NOT NULL",
            Label = "پیام")]
        public string Message { get; set; }

        /*public Log() : this(true) { }

        public Log(bool setDefaults)
        {
            if (setDefaults)
            {
                DateTime = System.DateTime.Now;
            }
        }*/
    }
}
