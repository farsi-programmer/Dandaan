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
    [Dandaan(Label = "فیلدها", EnableAdd = true, EnableDelete = true, EnableSearch = true, EnableEdit = true)]
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

        [Column(IsPrimaryKey = true)]
        [DandaanColumn(Sql = "[int] NOT NULL CONSTRAINT [FK_" + nameof(Column) + "_" + nameof(UserTable) + @"]
FOREIGN KEY REFERENCES [dbo].[" + nameof(UserTable) + "] ([" + nameof(UserTable.Id) + "])",
            //ForeignTable = typeof(UserTable),
            ForeignTableDisplayColumn = nameof(UserTable.Label),
            Label = "فرم")]
        public int? UserTableId { get; set; }

        //[Column]
        //[DandaanColumn(Sql = "[nvarchar](100) NOT NULL",
        //    Label = "نوع")]
        //public string Type { get; set; }

        [Column]
        [DandaanColumn(Sql = "[int] NOT NULL",
            Label = "نوع")]
        public ColumnType? Type { get; set; }

        //[Column]
        //[DandaanColumn(Sql = "[int] NOT NULL")]
        //public NoOrYes Nullable { get; set; }

        //[Column]
        //[DandaanColumn(Sql = "[nvarchar](1000) NOT NULL",
        //    Label = "مقدار پیش فرض")]
        //public object DefaultValue { get; set; }

        //[Column]
        //[DandaanColumn(Sql = "[int] NOT NULL",
        //    Label = "امکان اضافه")]
        //public NoOrYes? EnableAdd { get; set; }

        //[Column]
        //[DandaanColumn(Sql = "[int] NOT NULL",
        //    Label = "امکان حذف")]
        //public NoOrYes? EnableDelete { get; set; }

        //[Column]
        //[DandaanColumn(Sql = "[int] NOT NULL",
        //    Label = "امکان ویرایش")]
        //public NoOrYes? EnableEdit { get; set; }

        //[Column]
        //[DandaanColumn(Sql = "[int] NOT NULL",
        //    Label = "امکان جستجو")]
        //public NoOrYes? EnableSearch { get; set; }

        //[Column]
        //[DandaanColumn(Sql = "[int] NOT NULL")]
        //public SqlDbType SqlType { get; set; }

        //[Column]
        //[DandaanColumn(Sql = "[int] NOT NULL")]
        //public int Length { get; set; }

        //[Column]
        //[DandaanColumn(Sql = "[int] NOT NULL")]
        //public NoOrYes Null { get; set; }

        [Column]
        [DandaanColumn(Sql = "[int] NOT NULL",
            Label = "الزامی")] // حتما باید وارد شود
        public YesOrNo? NotNull { get; set; } = YesOrNo.خیر;

        //[Column]
        //[DandaanColumn(Sql = "[int] NOT NULL")]
        //public NoOrYes Identity { get; set; }

        //[Column]
        //[DandaanColumn(Sql = "[int] NOT NULL")]
        //public NoOrYes PrimaryKey { get; set; }

        // بدون تکرار
        //[Column]
        //[DandaanColumn(Sql = "[int] NOT NULL",
        //    Label = "یکتا")]
        //public NoOrYes? Unique { get; set; }

        // فیلد خارجی
        // فیلد از فرمی دیگر
        // کپی
        [Column]
        [DandaanColumn(Sql = "[int] CONSTRAINT [FK_" + nameof(Column) + "_" + nameof(Column) + @"]
FOREIGN KEY REFERENCES [dbo].[" + nameof(Column) + "] ([" + nameof(Id) + "])",
            ForeignTableDisplayColumn = nameof(Label),
            Label = "فیلد مرجع")]
        public int? ReferenceColumnId { get; set; }
    }
    
    public enum ColumnType
    {
        //متن, متن_بدون_تکرار,
        متن_تک_خطی, متن_تک_خطی_بدون_تکرار, متن_چند_خطی, متن_چند_خطی_بدون_تکرار,
        //عدد, عدد_بدون_تکرار,
        عدد_صحیح, عدد_صحیح_بدون_تکرار, عدد_اعشاری, عدد_اعشاری_بدون_تکرار,
        //تاریخ, تاریخ_بدون_تکرار,
        تاریخ_با_دقت_دقیقه, تاریخ_با_دقت_دقیقه_بدون_تکرار, تاریخ_با_دقت_ثانیه, تاریخ_با_دقت_ثانیه_بدون_تکرار,
        تاریخ_با_دقت_دقیقه_با_مقدار_پیش_فرض_اکنون, تاریخ_با_دقت_دقیقه_بدون_تکرار_با_مقدار_پیش_فرض_اکنون, تاریخ_با_دقت_ثانیه_با_مقدار_پیش_فرض_اکنون, تاریخ_با_دقت_ثانیه_بدون_تکرار_با_مقدار_پیش_فرض_اکنون,
        فایل,
        //ارجاع
        //کپی_فیلد_مرجع
    }
}