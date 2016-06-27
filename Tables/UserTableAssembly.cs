using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq.Mapping;
using System.Text;
using System.Threading.Tasks;

namespace Dandaan.Tables
{
    [Table(Name = nameof(UserTableAssembly))]
    [Dandaan(Label = "اسمبلی​ها", EnableAdd = true, EnableDelete = true, EnableSearch = true, EnableEdit = true)]
    public class UserTableAssembly
    {
        [Column(IsPrimaryKey = true)]
        [DandaanColumn(Sql = "[int] NOT NULL CONSTRAINT [PK_" + nameof(UserTableAssembly) + @"]
PRIMARY KEY CLUSTERED CONSTRAINT [FK_" + nameof(UserTableAssembly) + "_" + nameof(UserTable) + @"]
FOREIGN KEY REFERENCES [dbo].[" + nameof(UserTable) + "] ([" + nameof(UserTable.Id) + "])",
            Label = "فرم")]
        public int? UserTableId { get; set; }
        
        [Column]
        [DandaanColumn(Sql = "[varbinary](max) NOT NULL",
            Label = "اسمبلی")]
        public byte[] Assembly { get; set; }
    }
}
