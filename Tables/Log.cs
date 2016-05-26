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
    public class Log
    {
        [Column]//(IsPrimaryKey = true, IsDbGenerated = true)]
        [Dandaan(Sql = "[int] IDENTITY NOT NULL CONSTRAINT [PK_" + nameof(Log) + "] PRIMARY KEY CLUSTERED (Id DESC)")]
        public int Id { get; set; }

        [Column]
        [Dandaan(Sql = "[nvarchar](1000) NOT NULL")]
        public string Message { get; set; }

        [Column]//(IsDbGenerated = true)]
        [Dandaan(Sql = "[smalldatetime] NOT NULL CONSTRAINT [DF_" + nameof(Log) + "_" + nameof(DateTime) + "] DEFAULT (getdate())")]
        public DateTime DateTime { get; set; }
    }
}
