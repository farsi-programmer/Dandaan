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
    [Table(Name = nameof(Column))]
    [Dandaan(Label = "مشخصات")]
    public class Column
    {
        [Column]
        [DandaanColumn(Sql = "[int] IDENTITY NOT NULL CONSTRAINT [PK_" + nameof(Column) + @"]
PRIMARY KEY CLUSTERED (Id DESC)",
            Label = "شماره")]
        public int? Id { get; set; }

        [Column]
        [DandaanColumn(Sql = "[nvarchar](100) NOT NULL")]
        public string Type { get; set; }

        [Column]
        [DandaanColumn(Sql = "[nvarchar](100) NOT NULL",
            Label = "نام")]
        public string Name { get; set; }

        [Column]
        [DandaanColumn(Sql = "[nvarchar](1000) NOT NULL",
            Label = "مقدار پیش فرض")]
        public object DefaultValue { get; set; }

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

        [Column]
        [DandaanColumn(Sql = "[int] NOT NULL")]
        public SqlDbType SqlType { get; set; }

        [Column]
        [DandaanColumn(Sql = "[int] NOT NULL")]
        public int Length { get; set; }

        [Column]
        [DandaanColumn(Sql = "[tinyint] NOT NULL")]
        public byte Null { get; set; }

        [Column]
        [DandaanColumn(Sql = "[tinyint] NOT NULL")]
        public byte Identity { get; set; }

        [Column]
        [DandaanColumn(Sql = "[tinyint] NOT NULL")]
        public byte PrimaryKey { get; set; }

        [Column]
        [DandaanColumn(Sql = "[tinyint] NOT NULL")]
        public byte Unique { get; set; }

        [Column]
        [DandaanColumn(Sql = "[int] NOT NULL CONSTRAINT [FK_" + nameof(Column) + "_" + nameof(UserTable) + @"]
FOREIGN KEY REFERENCES [dbo].[" + nameof(UserTable) + "] ([" + nameof(UserTable.Id) + "])")]
        public int UserTableId { get; set; }
    }
}