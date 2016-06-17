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
    class Foreignkey
    {
        [Column]
        [DandaanColumn(Sql = "[int] NOT NULL CONSTRAINT [FK_" + nameof(Foreignkey) + "_" + nameof(Column) + @"]
FOREIGN KEY REFERENCES [dbo].[" + nameof(Column) + "] ([" + nameof(Column.Id) + "])")]
        public int ColumnId { get; set; }

        [Column]
        [DandaanColumn(Sql = "[int] NOT NULL CONSTRAINT [FK_" + nameof(Foreignkey) + "_" + nameof(Column) + "_2"
+ @"] FOREIGN KEY REFERENCES [dbo].[" + nameof(Column) + "] ([" + nameof(Column.Id) + "])")]
        public int ReferenceColumnId { get; set; }
    }
}
