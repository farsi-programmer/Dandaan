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
    [Dandaan(Label = "داده​ها", EnableAdd = true, EnableDelete = true, EnableSearch = true, EnableEdit = true)]
    public class Column
    {
        [Column]
        [DandaanColumn(Sql = "[int] IDENTITY NOT NULL CONSTRAINT [IX_" + nameof(Column) + @"]
UNIQUE NONCLUSTERED",
            Label = "شماره")]
        public int? Id { get; set; }

        //[Column]
        //[DandaanColumn(Sql = "[nvarchar](100) NOT NULL",
        //    Label = "نام")]
        //public string Name { get; set; }

        [Column(IsPrimaryKey = true)]
        [DandaanColumn(Sql = "[nvarchar](100) NOT NULL CONSTRAINT [PK_" + nameof(Column) + @"]
PRIMARY KEY CLUSTERED ([Label] ASC, [UserTableId] ASC)",
            Label = "نام")]
        public string Label { get; set; }

        [Column]
        [DandaanColumn(Sql = "[nvarchar](100) NOT NULL")]
        public string Type { get; set; }

        [Column]
        [DandaanColumn(Sql = "[int] NOT NULL")]
        public NoOrYes Nullable { get; set; }

        [Column]
        [DandaanColumn(Sql = "[nvarchar](1000) NOT NULL",
            Label = "مقدار پیش فرض")]
        public object DefaultValue { get; set; }

        [Column]
        [DandaanColumn(Sql = "[int] NOT NULL",
            Label = "امکان اضافه")]
        public NoOrYes EnableAdd { get; set; }

        [Column]
        [DandaanColumn(Sql = "[int] NOT NULL",
            Label = "امکان حذف")]
        public NoOrYes EnableDelete { get; set; }

        [Column]
        [DandaanColumn(Sql = "[int] NOT NULL",
            Label = "امکان ویرایش")]
        public NoOrYes EnableEdit { get; set; }

        [Column]
        [DandaanColumn(Sql = "[int] NOT NULL",
            Label = "امکان جستجو")]
        public NoOrYes EnableSearch { get; set; }

        [Column]
        [DandaanColumn(Sql = "[int] NOT NULL")]
        public SqlDbType SqlType { get; set; }

        [Column]
        [DandaanColumn(Sql = "[int] NOT NULL")]
        public int Length { get; set; }

        [Column]
        [DandaanColumn(Sql = "[int] NOT NULL")]
        public NoOrYes Null { get; set; }

        [Column]
        [DandaanColumn(Sql = "[int] NOT NULL")]
        public NoOrYes Identity { get; set; }

        [Column]
        [DandaanColumn(Sql = "[int] NOT NULL")]
        public NoOrYes PrimaryKey { get; set; }

        [Column]
        [DandaanColumn(Sql = "[int] NOT NULL")]
        public NoOrYes Unique { get; set; }

        [Column(IsPrimaryKey = true)]
        [DandaanColumn(Sql = "[int] NOT NULL CONSTRAINT [FK_" + nameof(Column) + "_" + nameof(UserTable) + @"]
FOREIGN KEY REFERENCES [dbo].[" + nameof(UserTable) + "] ([" + nameof(UserTable.Id) + "])")]
        public int UserTableId { get; set; }

        [Column]
        [DandaanColumn(Sql = "[int] CONSTRAINT [FK_" + nameof(Column) + "_" + nameof(Column) + @"]
FOREIGN KEY REFERENCES [dbo].[" + nameof(Column) + "] ([" + nameof(Id) + "])")]
        public int? ReferenceColumnId { get; set; }
    }
}