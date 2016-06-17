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
    [Table(Name = nameof(UserTable))]
    [Dandaan(Label = "فرمها")]
    public class UserTable
    {
        [Column]
        [DandaanColumn(Sql = "[int] IDENTITY NOT NULL CONSTRAINT [PK_" + nameof(UserTable) + @"]
PRIMARY KEY CLUSTERED (Id DESC)",
            Label = "شماره")]
        public int? Id { get; set; }

        [Column]
        [DandaanColumn(Sql = "[nvarchar](100) NOT NULL",
            Label = "نام")]
        public string Name { get; set; }

        [Column]
        [DandaanColumn(Sql = "[nvarchar](100) NOT NULL",
            Label = "نام")]
        public string Label { get; set; }

        [Column]
        [DandaanColumn(Sql = "[tinyint] NOT NULL",
            Label = "امکان اضافه")]
        public byte EnableAdd { get; set; }

        [Column]
        [DandaanColumn(Sql = "[tinyint] NOT NULL",
            Label = "امکان حذف")]
        public byte EnableDelete { get; set; }

        [Column]
        [DandaanColumn(Sql = "[tinyint] NOT NULL",
            Label = "امکان ویرایش")]
        public byte EnableEdit { get; set; }

        [Column]
        [DandaanColumn(Sql = "[tinyint] NOT NULL",
            Label = "امکان جستجو")]
        public byte EnableSearch { get; set; }
    }
}
