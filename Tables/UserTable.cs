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
    [Dandaan(Label = "فرم​ها", EnableAdd = true, EnableDelete = true, EnableSearch = true, EnableEdit = true)]
    public class UserTable
    {
        [Column]
        [DandaanColumn(Sql = "[int] IDENTITY NOT NULL CONSTRAINT [IX_" + nameof(UserTable) + @"]
UNIQUE NONCLUSTERED",
            Label = "شماره")]
        public int? Id { get; set; }

        //[Column]
        //[DandaanColumn(Sql = "[nvarchar](100) NOT NULL",
        //    Label = "نام")]
        //public string Name { get; set; }

        [Column(IsPrimaryKey = true)]
        [DandaanColumn(Sql = "[nvarchar](100) NOT NULL CONSTRAINT [PK_" + nameof(UserTable) + @"]
PRIMARY KEY CLUSTERED (Label DESC)",
            Label = "نام")]
        public string Label { get; set; }

        [Column]
        [DandaanColumn(Sql = "[int] NOT NULL",
            Label = "امکان اضافه")]
        public YesOrNo? EnableAdd { get; set; }

        [Column]
        [DandaanColumn(Sql = "[int] NOT NULL",
            Label = "امکان حذف")]
        public YesOrNo? EnableDelete { get; set; }

        [Column]
        [DandaanColumn(Sql = "[int] NOT NULL",
            Label = "امکان ویرایش")]
        public YesOrNo? EnableEdit { get; set; }

        [Column]
        [DandaanColumn(Sql = "[int] NOT NULL",
            Label = "امکان جستجو")]
        public YesOrNo? EnableSearch { get; set; }
    }

    //public enum NoOrYes { No, Yes }
    public enum YesOrNo { بله, خیر }
}
